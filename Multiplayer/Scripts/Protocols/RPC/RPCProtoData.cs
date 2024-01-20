using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Multiplayer.Scripts.Protocols.RPC
{
    internal class RPCProtoData
    {
        public string Name { get; set; }

        public object[] Args { get; set; }

        public RPCProtoData(byte[] data)
        {
            var type = data[0];
        }
    }
}
