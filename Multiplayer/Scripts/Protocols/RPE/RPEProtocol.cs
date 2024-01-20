using System.Net;
using UpdServerCore.Core;

namespace UpdServerCore.Protocols.RPE
{
    public class RPEProtocol : IProtocol
    {
        public byte Proto => 0x1;

        private byte Command = 0x0;

        public RPEProtocol(RPECommandData command) 
        { 
            Command = (byte)command;
        }

        public static bool Check(byte[] bytes) => bytes.Length == 2;

        public bool CheckBuffer(byte[] bytes)
        {
            return Check(bytes);
        }

        public byte[] GetBuffers()
        {
            var buffers = new byte[2];
            buffers[0] = Proto;
            buffers[1] = Command;

            return buffers;
        }

        public void SendTo(IUpdInstance udpInstance, EndPoint endPoint)
        {
            var buffers = GetBuffers();

            udpInstance.SendTo(buffers, endPoint);
        }

        public static bool TryParse(byte[] bytes, out RPECommandData commandData)
        {
            if(bytes[0] != 0x1)
            {
                commandData = RPECommandData.None;
                return false;
            }

            try
            {
                commandData = (RPECommandData)bytes[1];

                return true;
            }
            catch
            {
                commandData = RPECommandData.Error;
                return false;
            }
        }
    }
}
