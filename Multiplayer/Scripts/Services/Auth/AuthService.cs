using System.Net;
using System.Threading.Tasks;
using System.Threading;
using UdpServerCore.Core;
using UdpServerCore.Protocols.RPE;
using UnityEngine;
using Assets.Multiplayer.Scheduler;

namespace Multiplayer.Scripts.Services.Auth
{
	public sealed class AuthService
	{
		private INetworkService _updInstance;
		private INetworkScheduler _networkScheduler;
		private bool _isConnections;

		public AuthService(
			INetworkService networkService,
			INetworkScheduler networkScheduler)
		{
			_networkScheduler = networkScheduler;
			_updInstance      = networkService;
		}

		public bool TryConnect(string ip, int port)
		{
			var address = new IPEndPoint(IPAddress.Parse(ip), port);
			try
			{
				using(var cancel = new CancellationTokenSource(System.TimeSpan.FromSeconds(15)))
				{
					var task = Task.Run(() =>
					{
						do
						{
							if(cancel.Token.IsCancellationRequested)
							{
								cancel.Token.ThrowIfCancellationRequested();
							}

							var startProto = new RPEProtocol(RPECommandData.Subsribe);
							startProto.SendTo((UpdInstance)_updInstance, address);

							Thread.Sleep(500);
						}
						while(!_isConnections);
					}, cancel.Token);

					task.Wait();

					if(task.Status == TaskStatus.Canceled)
					{
						Debug.LogError("Сервер не найден!");
						return false;
					}
					else
					{
						_networkScheduler.TryConnect(address);
					}

					return true;
				}
			}
			catch
			{
				Debug.LogError("Сервер не найден!");
				return false;
			}
		}
	}
}
