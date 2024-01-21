using System.Net;
using UdpServerCore.Core;
using UdpServerCore.Framework;
using UdpServerCore.Protocols.RPE;
using UdpServerCore.Protocols.Sync;
using UdpServerCore.Protocols;
using System.Linq;
using Assets.Multiplayer.Framework;
using UnityEngine;
using Assets.Multiplayer.Serilizator;
using System.Threading;
using System.Threading.Tasks;
using Assets.Multiplayer.Scheduler;
using Assets.Multiplayer.Scripts.Protocols.RPC;
using Assets.Multiplayer.Scripts.Framework;
using Multiplayer.Scripts.Services.Auth;

namespace UdpServerCore.Servers
{
	/// <summary>Сервер чтения и прослушивания sync протоколов.</summary>
	public sealed class SyncHandler
	{
		private const int StdLen = 2048;
		private INetworkService _updInstance;
		private bool _isDebug;
		private INetworkScheduler _networkScheduler;

		private AuthService AuthService { get; }

		private SynchronizationContext Context { get; }
		public SyncMainContainer Containers { get; private set; }

		private bool _isConnections;

		private IIPEndPointClient iPEndPointClient { get; }

		public bool IsServer { get; private set; }

		public SyncHandler(
			INetworkService updInstance,
			SynchronizationContext synchronizationContext,
			SyncMainContainer syncMainContainer,
			AuthService authService,
			INetworkScheduler networkScheduler,

			bool isDebug = false) 
		{
			_networkScheduler = networkScheduler;
			_updInstance = updInstance;
			Containers = syncMainContainer;
			Context = synchronizationContext;
			AuthService = authService;

			_isDebug = isDebug;
		}

		public void StartClient(int port)
		{
			_updInstance.Initialize();
			_updInstance.Start(IPAddress.Any, port, StdLen);
			_updInstance.EventHandler += OnEvent;

			IsServer = false;
		}

		public void StartServer(string ip, int port)
		{
			_updInstance.Initialize();
			_updInstance.Start(IPAddress.Parse(ip), port, StdLen);
			_updInstance.EventHandler += OnEvent;

			IsServer = true;
		}

		public bool TryConnect(string ip, int port)
			=> AuthService.TryConnect(ip, port);
		

		private void OnEvent(object sender, ResponseData responseData)
		{
			if(RPEProtocol.TryParse(responseData.Data, out RPECommandData rpeCom))
			{
				AuthService.CallResponse(responseData, rpeCom, _isDebug);
			}
			else if(SyncProtocol.TryParse(responseData.Data, out ISyncData syncData))
			{
				if(IsServer && !_networkScheduler.Check(responseData.EndPoint))
				{
					return;
				}

				var page = Containers.BehaviorContainers.FirstOrDefault(x => x.Token == syncData.TokenSync);
				if(page == null)
				{
					return;
				}

				var field = page.Fields.FirstOrDefault(x => x.Id == syncData.FieldId);

				if((byte)syncData.Type == TypeTable.TransformTypeByte)
				{
					var transformData = TransformSerialize.Deserialize(syncData.FieldData);

					Context.Post(d =>
					{
						page.SetTransform(transformData);
					}, null);
				}
				else
				{
					var val = TypeTable.ConvertDataToObject((byte)syncData.Type, syncData.FieldData);
					page.Set(field, val);
				}
			}
			else if(RPCProto.TryParse(responseData.Data, out RPCProtoData protoData))
			{
				protoData.Sender = responseData.EndPoint;
				FuncHandleApi.Call(protoData);

				Debug.Log($"{protoData.Name}");
			}
		}
	}
}
