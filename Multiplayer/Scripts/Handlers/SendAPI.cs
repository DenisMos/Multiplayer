using Assets.Module.Multiplayer.Scripts.Services;
using Assets.Multiplayer.Scripts.Protocols.RPC;
using System;
using UdpServerCore.Core;
using UdpServerCore.Framework;
using UdpServerCore.Protocols;

namespace Multiplayer.Scripts.Handlers
{
	public sealed class SendAPI : INetService<RPCProtoData>
	{
		private INetworkService _updInstance;
		private IIPEndPointClient _iPEndPointClient;

		public SendAPI(
			INetworkService updInstance,
			IIPEndPointClient iPEndPointClient) 
		{
			_updInstance = updInstance;
			_iPEndPointClient= iPEndPointClient;
		}

		public void SendBytes(Func<IProtocol> func)
		{
			var proto = func.Invoke();
			foreach(var client in _iPEndPointClient.EndPoints)
			{
				proto.SendTo(_updInstance, client);
			}
		}

		internal void Call(RPCProtoData rPCProtoData)
		{
			if(rPCProtoData.Name == "GetUserSocket") 
			{
				_iPEndPointClient.userTableRule.Rules
					.Add(rPCProtoData.Args[0].ToString(), rPCProtoData.Sender);
			}

			UnityEngine.Debug.Log(rPCProtoData.Name);
		}

		public void CallResponse(ResponseData responseData, RPCProtoData data, bool verb)
		{
			Call(data);
		}
	}
}
