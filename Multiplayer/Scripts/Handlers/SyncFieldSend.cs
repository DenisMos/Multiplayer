using Assets.Multiplayer.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UpdServerCore.Core;
using UpdServerCore.Protocols.Sync;
using UpdServerCore.Servers;
using UpdServerCore.Framework;

using Assets.Multiplayer.Serilizator;
using Assets.Multiplayer.Attributes;
using Assets.Multiplayer.Scheduler;

namespace UpdServerCore.Clients
{
	/// <summary>Рассылка протоколов.</summary>
	public sealed class SyncFieldSend
	{
		public bool Status { get; set; }

		public int Rate { get; set; } = 10;

		private INetworkScheduler _networkScheduler;

		private Task Task { get; set; }

		public SyncFieldSend(
			INetworkScheduler networkScheduler)
		{
			_networkScheduler = networkScheduler;
		}

		public void Stop()
		{
			Status = false;
		}

		public void StartSending()
		{
			Task = Task.Run(() => 
			{
                Debug.Log("Сервис синхронизации начал работу.");

                while(Status)
				{
					_networkScheduler.Resolve();

					Thread.Sleep(Rate);
				}

				Debug.Log("Сервис синхронизации прекратил работу.");
			});
		}
	}
}
