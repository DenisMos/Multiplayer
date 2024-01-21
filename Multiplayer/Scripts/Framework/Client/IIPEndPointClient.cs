using Assets.Module.Multiplayer.Scripts.Framework;
using Assets.Multiplayer.Scheduler;
using System.Collections.Generic;
using System.Net;

namespace UdpServerCore.Framework
{
    /// <summary>Таблица подключённых клиентов.</summary>
    public interface IIPEndPointClient
    {
        List<EndPoint> EndPoints { get; }

        UserTableRule userTableRule { get; }
        void AddConnect(EndPoint endPoint);

        void Disconnect(EndPoint endPoint);

        EndPoint[] GetEndPoint(EndPoint endPoint);

        bool Check(EndPoint endPoint);
    }
}
