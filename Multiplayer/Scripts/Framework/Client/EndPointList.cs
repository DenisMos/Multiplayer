using Assets.Module.Multiplayer.Scripts.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace UdpServerCore.Framework.ClientList
{
	public sealed class EndPointList : IIPEndPointClient
	{
		public List<EndPoint> EndPoints { get; }

		public UserTableRule userTableRule { get; }

		public EndPointList()
		{
			EndPoints = new List<EndPoint>();
			userTableRule = new UserTableRule();
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
