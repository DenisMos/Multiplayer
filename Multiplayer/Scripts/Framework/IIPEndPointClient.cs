using System.Collections.Generic;
using System.Net;

namespace UpdServerCore.Framework
{
    /// <summary>Таблица подключённых клиентов.</summary>
    public interface IIPEndPointClient
    {
        List<EndPoint> EndPoints { get; }

        void AddConnect(EndPoint endPoint);

        void Disconnect(EndPoint endPoint);

        EndPoint[] GetEndPoint(EndPoint endPoint);

        bool Check(EndPoint endPoint);
    }
}
