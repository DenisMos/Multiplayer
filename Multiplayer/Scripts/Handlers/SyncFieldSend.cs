﻿using Assets.Multiplayer.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UdpServerCore.Core;
using UdpServerCore.Protocols.Sync;
using UdpServerCore.Servers;
using UdpServerCore.Framework;

using Assets.Multiplayer.Serilizator;
using Assets.Multiplayer.Attributes;
using Assets.Multiplayer.Scheduler;
using Assets.Multiplayer.Scripts.Converts;
using System.Net;
using System.Collections.Generic;
using Assets.Multiplayer.Scripts.Extansions;
using UdpServerCore.Protocols;
using Assets.Module.Multiplayer.Scripts.Scheduler;
using Multiplayer;

namespace UdpServerCore.Clients
{
	/// <summary>Рассылка протоколов.</summary>
	public sealed class SyncFieldSend
	{
		public bool Status { get; set; }

		public int Rate { get; set; } = 10;

		private INetworkScheduler _networkScheduler;
		private IIPEndPointClient _iPEndPointClient1;
		private SyncMainContainer _syncMainContainer;
		private INetworkService _updInstance;

		private Dictionary<FieldData, byte[]> CacheFields = new Dictionary<FieldData, byte[]>();

		private Task Task { get; set; }

		public SyncFieldSend(
			INetworkScheduler networkScheduler,
			IIPEndPointClient iPEndPointClient,
			SyncMainContainer syncMainContainer,
			INetworkService updInstance)
		{
			_networkScheduler = networkScheduler;
			_iPEndPointClient1 = iPEndPointClient;
			_syncMainContainer = syncMainContainer;
			_updInstance = updInstance;
		}

		public void Stop()
		{
			Status = false;
		}

		private IProtocol Resolve(EndPoint endPoint)
		{
			foreach(var bhContainer in _syncMainContainer.BehaviorContainers)
			{
				if(_networkScheduler.IsServer && _iPEndPointClient1.userTableRule.Rules.ContainsKey(bhContainer.Token.ToString()))
					continue;

				if(!_networkScheduler.IsServer && !_iPEndPointClient1.userTableRule.Rules.ContainsKey(bhContainer.Token.ToString()))
					continue;

				foreach(var field in bhContainer.Fields)
				{
					if(field.Value == null && field.Id >= 0) continue;

					try
					{
						var data = ConvertProtoData.Convert(field, bhContainer, null);

						CacheFields[field] = data;

						var protocol = field.ToProto(bhContainer.Token, data);
						protocol.SendTo(_updInstance, endPoint);
					}
					catch(Exception exc)
					{
						Debug.LogError(exc);
					}
				}
			}

			return null;
		}

		private bool EquelsCache(FieldData fieldData, byte[] data)
		{
			if(CacheFields.ContainsKey(fieldData))
			{
				return CacheFields[fieldData].EqualsArray(data);
			}

			CacheFields.Add(fieldData, data);
			return true;
		}

		public void StartSending()
		{
			Task = Task.Run(() =>
			{
				NetworkLogger.Log("Сервис синхронизации начал работу.");

				_networkScheduler.Post(
				   new QueueItemNet(
					  () => {
						  foreach(var item in _iPEndPointClient1.EndPoints.ToArray())
						  {
							  Resolve(item);
						  }
					  }, RulesSending.Cycle, "sendingFields"
				   )
				);

				while(Status)
				{
					_networkScheduler.Resolve();
					Thread.Sleep(Rate);
				}

				NetworkLogger.Log("Сервис синхронизации прекратил работу.");
			});
		}
	}
}
