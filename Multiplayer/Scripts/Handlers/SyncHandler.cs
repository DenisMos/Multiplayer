using System.Net;
using UpdServerCore.Core;
using UpdServerCore.Framework;
using UpdServerCore.Framework.ClientList;
using UpdServerCore.Protocols.RPE;
using UpdServerCore.Protocols.Sync;
using UpdServerCore.Protocols;
using System.Linq;
using Assets.Multiplayer.Framework;
using UnityEngine;
using Assets.Multiplayer.Serilizator;
using System.Threading;
using System.Threading.Tasks;
using Assets.Multiplayer.Scheduler;
using Assets.Multiplayer.Scripts.Protocols.RPC;
using System;
using Assets.Multiplayer.Scripts.Framework;

namespace UpdServerCore.Servers
{
	/// <summary>Сервер чтения и прослушивания sync протоколов.</summary>
	public sealed class SyncHandler
	{
		private const int StdLen = 2048;
		private IUpdInstance _updInstance;
		private bool _isDebug;
		private INetworkScheduler _networkScheduler;

		private SynchronizationContext Context { get; }
		public SyncMainContainer Containers { get; private set; }

		private bool _isConnections;

		private IIPEndPointClient iPEndPointClient { get; }

        public bool IsServer { get; private set; }

		public SyncHandler(
			IUpdInstance updInstance,
			SynchronizationContext synchronizationContext,
			SyncMainContainer syncMainContainer,
            IIPEndPointClient endPoint,
            INetworkScheduler networkScheduler,

			bool isDebug = false) 
		{
			_networkScheduler = networkScheduler;
			_updInstance = updInstance;
			Containers = syncMainContainer;
			Context = synchronizationContext;
            iPEndPointClient = endPoint;

            _isDebug = isDebug;
		}

		public void Initialization(SyncMainContainer container)
		{
			Containers = container;
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
		{
			var address = new IPEndPoint(IPAddress.Parse(ip), port);
			try
			{
				using(var cancel = new CancellationTokenSource(System.TimeSpan.FromSeconds(15)))
				{
					var task = Task.Run(() =>
					{
						do
						{
							if(cancel.Token.IsCancellationRequested)
							{
								cancel.Token.ThrowIfCancellationRequested();
							}

							var startProto = new RPEProtocol(RPECommandData.Subsribe);
							startProto.SendTo((UpdInstance)_updInstance, address);

							Thread.Sleep(500);
						} 
						while(!_isConnections);
					}, cancel.Token);

					task.Wait();

					if(task.Status == TaskStatus.Canceled)
					{
						Debug.LogError("Сервер не найден!");
						return false;
					}
					else
					{ 
						_networkScheduler.TryConnect(address);
					}

					return true;
				}
			}
			catch
			{
				Debug.LogError("Сервер не найден!");
				return false;
			}
		}

		private void OnEvent(object sender, ResponseData responseData)
		{
			if(RPEProtocol.TryParse(responseData.Data, out RPECommandData rpeCom))
			{
				switch(rpeCom)
				{
					case RPECommandData.Subsribe:
						if(_networkScheduler.TryConnect(responseData.EndPoint))
						{
							if(_isDebug)
							{
								Debug.Log($"{responseData.EndPoint} - connected");
							}
						}
						var startProto = new RPEProtocol(RPECommandData.Start);
						startProto.SendTo((UpdInstance)_updInstance, responseData.EndPoint);

						//_networkScheduler.Resolve(target: responseData.EndPoint); // Получение первичной синхронизации

						break;
					case RPECommandData.Exit:
						if(_networkScheduler.TryDisconnect(responseData.EndPoint))
						{
							if(_isDebug)
							{
								Debug.Log($"{responseData.EndPoint} - disconnected");
							}
						}
						break;
					case RPECommandData.Start:
						_isConnections = true;
						break;
					default:
						Debug.Log(rpeCom);
						break;
				}
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
