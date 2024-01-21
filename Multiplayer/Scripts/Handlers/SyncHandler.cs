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
using Multiplayer.Scripts.Services.Sync;

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

		private SyncService SyncService { get; }

		private SynchronizationContext Context { get; }

		public SyncMainContainer Containers { get; private set; }

		public bool IsServer { get; private set; }

		public SyncHandler(
			INetworkService updInstance,
			SynchronizationContext synchronizationContext,
			SyncMainContainer syncMainContainer,
			AuthService authService,
			SyncService syncService,
			INetworkScheduler networkScheduler,

			bool isDebug = false) 
		{
			_networkScheduler = networkScheduler;
			_updInstance = updInstance;
			Containers = syncMainContainer;
			Context = synchronizationContext;
			AuthService = authService;
			SyncService = syncService;

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
				SyncService.CallResponse(responseData, syncData);
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
