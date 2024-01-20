using System.Net;
using UpdServerCore.Core;

namespace UpdServerCore.Protocols
{
    public interface IProtocol
    {
        /// <summary>Тип протокола.</summary>
        public byte Proto { get; }

        void SendTo(IUpdInstance udpInstance, EndPoint endPoint);

        bool CheckBuffer(byte[] bytes);

        byte[] GetBuffers();

        //bool TryParse(byte[] buffers, out ISyncData syncData);
    }
}