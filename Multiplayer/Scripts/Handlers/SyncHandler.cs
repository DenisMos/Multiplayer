using System.Net;
using UdpServerCore.Core;
using UdpServerCore.Protocols.RPE;
using UdpServerCore.Protocols.Sync;
using UdpServerCore.Protocols;
using Assets.Multiplayer.Framework;
using Assets.Multiplayer.Scripts.Protocols.RPC;
using Assets.Multiplayer.Scripts.Framework;
using Multiplayer.Scripts.Services.Auth;
using Multiplayer.Scripts.Services.Sync;
using Multiplayer;
using System;

namespace UdpServerCore.Servers
{
	/// <summary>Сервер чтения и прослушивания протоколов.</summary>
	public sealed class SyncHandler
	{
		private const int StdLen = 2048;
		private INetworkService _udpInstance;
		private bool _isDebug;

		public bool IsServer { get; private set; }

		public IPEndPoint MyIp { get; private set; }

		private AuthService AuthService { get; }

		private SyncService SyncService { get; }

		public SyncMainContainer Containers { get; private set; }

		public SyncHandler(
			INetworkService udpInstance,
			SyncMainContainer syncMainContainer,
			AuthService authService,
			SyncService syncService,
			bool isDebug = false) 
		{
			_udpInstance = udpInstance;
			Containers = syncMainContainer;
			AuthService = authService;
			SyncService = syncService;

			_isDebug = isDebug;
		}

		public void StartClient(int port)
		{
			IsServer = false;

			var ip = IPAddress.Any;
			MyIp = new IPEndPoint(ip, port);

			_udpInstance.Initialize();
			_udpInstance.Start(ip, port, StdLen);
			_udpInstance.EventHandler += OnEvent;
		}

		public void StartServer(string ip, int port)
		{
			IsServer = true;

			MyIp = new IPEndPoint(IPAddress.Parse(ip), port);

			_udpInstance.Initialize();
			_udpInstance.Start(IPAddress.Parse(ip), port, StdLen);
			_udpInstance.EventHandler += OnEvent;
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

				NetworkLogger.Log($"{protoData.Name}");
			}
		}
	}
}
