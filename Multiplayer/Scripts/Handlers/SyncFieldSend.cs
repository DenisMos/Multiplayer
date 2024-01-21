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
using Assets.Multiplayer.Scripts.Converts;
using System.Net;
using System.Collections.Generic;
using Assets.Multiplayer.Scripts.Extansions;
using UpdServerCore.Protocols;
using Assets.Module.Multiplayer.Scripts.Scheduler;

namespace UpdServerCore.Clients
{
	/// <summary>Рассылка протоколов.</summary>
	public sealed class SyncFieldSend
	{
		public bool Status { get; set; }

		public int Rate { get; set; } = 10;

		private INetworkScheduler _networkScheduler;
		private IIPEndPointClient _iPEndPointClient1;
        private SyncMainContainer _syncMainContainer;
        private IUpdInstance _updInstance;

        private Dictionary<FieldData, byte[]> CacheFields = new Dictionary<FieldData, byte[]>();

        private Task Task { get; set; }

		public SyncFieldSend(
			INetworkScheduler networkScheduler,
			IIPEndPointClient iPEndPointClient,
            SyncMainContainer syncMainContainer,
            IUpdInstance updInstance)
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
            //if(!_networkScheduler.IsServer) return null;

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

                        if(!EquelsCache(field, data))
                        {
                            CacheFields[field] = data;

                            var protocol = field.ToProto(bhContainer.Token, data);
                            protocol.SendTo(_updInstance, endPoint);
                        }
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
                Debug.Log("Сервис синхронизации начал работу.");

                _networkScheduler.Post(
                   new QueueItemNet(
                      () => {
                          foreach(var item in _iPEndPointClient1.EndPoints)
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

				Debug.Log("Сервис синхронизации прекратил работу.");
			});
		}
	}
}
