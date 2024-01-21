using System.Net;
using UdpServerCore.Core;

namespace UdpServerCore.Protocols
{
    public interface IProtocol
    {
        /// <summary>Тип протокола.</summary>
        public byte Proto { get; }

        void SendTo(INetworkService udpInstance, EndPoint endPoint);

        bool CheckBuffer(byte[] bytes);

        byte[] GetBuffers();

        //bool TryParse(byte[] buffers, out ISyncData syncData);
    }
}