using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace UpdServerCore.Framework.ClientList
{
    public sealed class EndPointList : IIPEndPointClient
    {
        public List<EndPoint> EndPoints { get; }
     
        public EndPointList()
        {
            EndPoints = new List<EndPoint>();
        }

        public void AddConnect(EndPoint endPoint)
        {
            EndPoints.Add(endPoint);
        }

        public void Disconnect(EndPoint endPoint)
        {
            EndPoints.Remove(endPoint);
        }

        public EndPoint[] GetEndPoint(EndPoint endPoint)
        {
            return EndPoints.Where(x => x != endPoint).ToArray();
        }

        public bool Check(EndPoint endPoint)
        {
            return EndPoints.Contains(endPoint);
        }
    }
}
