using Assets.Multiplayer.Framework;
using Assets.Multiplayer.Scripts.Converts;
using Assets.Multiplayer.Scripts.Extansions;
using Assets.Multiplayer.Scripts.Protocols.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;
using UpdServerCore.Core;
using UpdServerCore.Framework;
using UpdServerCore.Framework.ClientList;
using UpdServerCore.Protocols.Sync;
using UpdServerCore.Servers;

namespace Assets.Multiplayer.Scheduler
{
	public sealed class NetworkScheduler : INetworkScheduler
	{
		public bool IsServer { get; }

		private IIPEndPointClient ClientTable { get; }

		private Dictionary<FieldData, byte[]> CacheFields = new Dictionary<FieldData, byte[]>();

		private Dictionary<EndPoint, NetworkUserRules> NetRules { get; }

		private SyncMainContainer SyncMainContainer { get; }

		private QueueNetModification QueueNetModification { get; }

		private SynchronizationContext Context { get; }

		private IUpdInstance UpdInstance { get; }


		private object _sync = new object();

		public NetworkScheduler(
			bool isServer,
			SyncMainContainer syncMainContainer,
			SynchronizationContext synchronizationContext,
			IUpdInstance updInstance)
		{
			IsServer = isServer;
			SyncMainContainer = syncMainContainer;
			Context = synchronizationContext;
			UpdInstance = updInstance;

			ClientTable = new EndPointList();
			NetRules = new Dictionary<EndPoint, NetworkUserRules>();
		}

		public void Dispose()
		{
			//throw new NotImplementedException();
		}

		public IEnumerable<IPageData<FieldData>> GetAllowFields()
		{
			throw new NotImplementedException();
		}

		public bool Check(EndPoint endPoint)
			=> ClientTable.Check(endPoint);

		public bool TryConnect(EndPoint endPoint)
		{
			lock(_sync)
			{
				if(!ClientTable.Check(endPoint))
				{
					ClientTable.AddConnect(endPoint);

					return true;
				}

				return false;
			}
		}

		public bool TryDisconnect(EndPoint endPoint) 
		{
			lock(_sync)
			{
				if(ClientTable.Check(endPoint))
				{
					ClientTable.Disconnect(endPoint);

					return true;
				}

				return false;
			}
		}

		private void Resolve(EndPoint ip, bool forced)
		{
			foreach(var bhContainer in SyncMainContainer.BehaviorContainers)
			{
				foreach(var field in bhContainer.Fields)
				{
					if(field.Value == null && field.Id >= 0) continue;

					try
					{
						var data = ConvertProtoData.Convert(field, bhContainer, Context);

						if(!EquelsCache(field, data) || forced)
						{
                            CacheFields[field] = data;

                            var syncProto = field.ToProto(bhContainer.Token, data);
							syncProto.SendTo(UpdInstance, ip);
						}
					}
					catch(Exception exc)
					{
						Debug.LogError(exc);
					}
				}
			}
		}

		/// <summary>Вызывает попытку разослать всем сообщения.</summary>
		/// <param name="ignorableUser"></param>
		/// <param name="target"></param>
		public void Resolve(EndPoint ignorableUser = null, EndPoint target = null)
		{
			lock(_sync)
			{
				if(target != null)
				{
					Resolve(target, forced: true);
				}
				else
				{
					foreach(var ip in ClientTable.EndPoints)
					{
						if(ignorableUser != null && ignorableUser.Equals(ip)) continue;

						Resolve(ip, forced: false);
					}
				}
			}
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

		/// <summary>Отправляет данные.</summary>
		/// <param name="data"></param>
		/// <param name="allClients"></param>
		public void Post(byte[] data)
		{
			Context.Post(x =>
			{
                lock(_sync)
                {
                    foreach(var ip in ClientTable.EndPoints)
                    {
                        var rpcProto = new RPCProto(data);
                        rpcProto.SendTo(UpdInstance, ip);
                    }
                }
            }, null);
        }

		public void Post(Func<byte[]> func, FieldData field, EndPoint sender)
		{
			lock(_sync)
			{
				if(field == null) return;

				var data = func.Invoke();

				if(IsServer)
				{
					Resolve(ignorableUser: sender);
				}

				CacheFields[field] = data;
			}
		}
	}
}
