using Assets.Module.Multiplayer.Scripts.Services;
using Assets.Multiplayer.Scripts.Protocols.RPC;
using System;
using UdpServerCore.Core;
using UdpServerCore.Framework;
using UdpServerCore.Protocols;
using UdpServerCore.Servers;

namespace Multiplayer.Scripts.Handlers
{
	public sealed class SendAPI : INetService<RPCProtoData>
	{
		private INetworkService _udpInstance;
		private IIPEndPointClient _iPEndPointClient;
		private SyncHandler _syncHandler;

		public SendAPI(
			INetworkService udpInstance,
			IIPEndPointClient iPEndPointClient,
			SyncHandler syncHandler) 
		{
			_udpInstance = udpInstance;
			_iPEndPointClient= iPEndPointClient;
			_syncHandler = syncHandler;
		}

		public void SendBytes(Func<IProtocol> func)
		{
			var proto = func.Invoke();
			foreach(var client in _iPEndPointClient.EndPoints)
			{
				proto.SendTo(_udpInstance, client);
			}
		}

		internal void Call(RPCProtoData rPCProtoData)
		{
			if(rPCProtoData.Name == "GetUserSocket") 
			{
				_iPEndPointClient.userTableRule.Rules
					.Add(rPCProtoData.Args[0].ToString(), rPCProtoData.Sender);
			}

			NetworkLogger.Log(rPCProtoData.Name);
		}

		public void CallResponse(ResponseData responseData, RPCProtoData data, bool verb)
		{
			Call(data);
		}
	}
}
