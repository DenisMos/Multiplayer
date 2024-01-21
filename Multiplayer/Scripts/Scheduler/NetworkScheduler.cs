using Assets.Module.Multiplayer.Scripts.Scheduler;
using Assets.Multiplayer.Framework;
using Assets.Multiplayer.Scripts.Converts;
using Assets.Multiplayer.Scripts.Extansions;
using Assets.Multiplayer.Scripts.Protocols.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using UnityEngine;
using UdpServerCore.Core;
using UdpServerCore.Framework;

namespace Assets.Multiplayer.Scheduler
{
	public sealed class NetworkScheduler : INetworkScheduler
	{
		public bool IsServer { get; set; }

		private IIPEndPointClient ClientTable { get; }

		/// <summary>Очередь планировщика.</summary>
		private Queue<QueueItemNet> Actions { get; } = new Queue<QueueItemNet>();


		private Dictionary<EndPoint, NetworkUserRules> NetRules { get; }

		private SyncMainContainer SyncMainContainer { get; }

		private SynchronizationContext Context { get; }

		private INetworkService UpdInstance { get; }


		private object _sync = new object();

		public NetworkScheduler(
			bool isServer,
			SyncMainContainer syncMainContainer,
			SynchronizationContext synchronizationContext,
			IIPEndPointClient iPEndPointClient,
			INetworkService updInstance)
		{
			IsServer = isServer;
			SyncMainContainer = syncMainContainer;
			Context = synchronizationContext;
			UpdInstance = updInstance;
			ClientTable = iPEndPointClient;
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

		//private void Resolve(EndPoint ip, bool forced)
		//{
		//	foreach(var bhContainer in SyncMainContainer.BehaviorContainers)
		//	{
		//		foreach(var field in bhContainer.Fields)
		//		{
		//			if(field.Value == null && field.Id >= 0) continue;

		//			try
		//			{
		//				var data = ConvertProtoData.Convert(field, bhContainer, Context);

		//				if(!EquelsCache(field, data) || forced)
		//				{
  //                          CacheFields[field] = data;

  //                          var syncProto = field.ToProto(bhContainer.Token, data);
		//					syncProto.SendTo(UpdInstance, ip);
		//				}
		//			}
		//			catch(Exception exc)
		//			{
		//				Debug.LogError(exc);
		//			}
		//		}
		//	}
		//}

		[Obsolete]
		/// <summary>Вызывает попытку разослать всем сообщения.</summary>
		/// <param name="ignorableUser"></param>
		/// <param name="target"></param>
		public void Resolve()
		{
			lock(_sync)
			{
				if(Actions.Count == 0) return;

				var deq = Actions.Dequeue();

				Context.Post(x =>
				{
					deq.Action.Invoke();
				}, null);

				if(deq.Type == RulesSending.Cycle)
				{
					Actions.Enqueue(deq);
				}
			}
		}

		public void Post(QueueItemNet queueItemNet)
		{
			var isContain = Actions.Any(x => x.Name == queueItemNet.Name);

			Context.Post(x =>
			{
				lock(_sync)
				{
					if(!isContain)
					{
						Actions.Enqueue(queueItemNet);
					}
				}
			}, null);
		}

		/// <summary>Отправляет данные.</summary>
		/// <param name="data"></param>
		/// <param name="allClients"></param>
		[Obsolete]
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

		//public void Post(Func<byte[]> func, FieldData field, EndPoint sender)
		//{
		//	lock(_sync)
		//	{
		//		if(field == null) return;

		//		var data = func.Invoke();

		//		if(IsServer)
		//		{
		//			Resolve(ignorableUser: sender);
		//		}

		//		//CacheFields[field] = data;
		//	}
		//}
	}
}
