using Assets.Multiplayer.Scheduler;
using Multiplayer.Scripts.Handlers;
using System.Collections.Generic;

namespace Assets.Multiplayer.Scripts.Framework
{
	internal class UdpInstanceAdapter
	{
		private List<string> MethodsList = new List<string>();

		public UdpInstanceAdapter(INetworkScheduler scheduler, SendAPI sendAPI)
		{
			FuncHandleApi.Initialize(scheduler, sendAPI);
		}
	}
}
