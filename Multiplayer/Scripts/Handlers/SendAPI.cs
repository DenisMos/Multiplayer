using Assets.Module.Multiplayer.Scripts.Framework;
using Assets.Multiplayer.Scripts.Protocols.RPC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UpdServerCore.Core;
using UpdServerCore.Framework;
using UpdServerCore.Protocols;

namespace Assets.Module.Multiplayer.Scripts.Handlers
{
    public sealed class SendAPI
    {
        private IUpdInstance _updInstance;
        private IIPEndPointClient _iPEndPointClient;

        public SendAPI(
            IUpdInstance updInstance,
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
    }
}
