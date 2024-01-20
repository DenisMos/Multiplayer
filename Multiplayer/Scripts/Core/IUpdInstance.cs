using System;
using System.Net;

namespace UpdServerCore.Core
{

    public interface IUpdInstance : IDisposable
    {
        int State { get; }

        string Name { get; }

        void Stop();

        void Start(IPAddress iPAddress, int port, int bufferLength);

        void Initialize();

        void SendTo(byte[] data, EndPoint endPoint);

        bool Disposed { get; }

        EventHandler<ResponseData> EventHandler { get; set; }
    }
}