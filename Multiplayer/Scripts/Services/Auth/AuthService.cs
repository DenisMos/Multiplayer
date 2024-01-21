using System.Net;
using System.Threading.Tasks;
using System.Threading;
using UdpServerCore.Core;
using UdpServerCore.Protocols.RPE;
using UnityEngine;
using Assets.Multiplayer.Scheduler;
using Assets.Module.Multiplayer.Scripts.Services;

namespace Multiplayer.Scripts.Services.Auth
{
	public sealed class AuthService : INetService<RPECommandData>
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

		/// <summary>Обрабатывает серверные команды.</summary>
		/// <param name="responseData"></param>
		/// <param name="rPECommandData"></param>
		/// <param name="verb"></param>
		public void CallResponse(
			ResponseData responseData, 
			RPECommandData rPECommandData,
			bool verb)
		{
			switch(rPECommandData)
			{
				case RPECommandData.Subsribe:
					if(_networkScheduler.TryConnect(responseData.EndPoint))
					{
						if(verb)
						{
							NetworkLogger.Log($"{responseData.EndPoint} - connected");
						}
					}
					var startProto = new RPEProtocol(RPECommandData.Start);
					startProto.SendTo((UpdInstance)_updInstance, responseData.EndPoint);

					break;
				case RPECommandData.Exit:
					if(_networkScheduler.TryDisconnect(responseData.EndPoint))
					{
						if(verb)
						{
							NetworkLogger.Log($"{responseData.EndPoint} - disconnected");
						}
					}
					break;
				case RPECommandData.Start:
					_isConnections = true;
					break;
				default:
					NetworkLogger.Log(rPECommandData);
					break;
			}
		}

		public bool TryConnect(string ip, int port)
		{
			NetworkLogger.Log($"Try connect: '{ip}:{port}'");

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

							NetworkLogger.Log($"reconnect: '{ip}:{port}'");
						}
						while(!_isConnections);
					}, cancel.Token);

					task.Wait();

					if(task.Status == TaskStatus.Canceled)
					{
						NetworkLogger.LogError("Сервер не найден!");
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
				NetworkLogger.LogError("Сервер не найден!");
				return false;
			}
		}
	}
}
