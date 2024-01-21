using Assets.Module.Multiplayer.Scripts.Services;
using Assets.Multiplayer.Framework;
using Assets.Multiplayer.Scheduler;
using Assets.Multiplayer.Serilizator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdpServerCore.Clients;
using UdpServerCore.Core;
using UdpServerCore.Protocols;

namespace Multiplayer.Scripts.Services.Sync
{
	public sealed class SyncService : INetService<ISyncData>
	{
		private bool IsServer { get; }

		private readonly INetworkScheduler _networkScheduler;
		private readonly SyncMainContainer Containers;

		public SyncService(
			INetworkScheduler networkScheduler, 
			SyncMainContainer containers,
			bool isServer) 
		{
			_networkScheduler = networkScheduler;
			Containers = containers;
			IsServer = isServer;
		}

		public void CallResponse(
			ResponseData responseData, 
			ISyncData syncData, 
			bool verb = false)
		{
			if(IsServer && !_networkScheduler.Check(responseData.EndPoint))
			{
				return;
			}

			var page = Containers.BehaviorContainers.FirstOrDefault(x => x.Token == syncData.TokenSync);
			if(page == null)
			{
				return;
			}

			var field = page.Fields.FirstOrDefault(x => x.Id == syncData.FieldId);

			if((byte)syncData.Type == TypeTable.TransformTypeByte)
			{
				var transformData = TransformSerialize.Deserialize(syncData.FieldData);

				_networkScheduler.Call(() => page.SetTransform(transformData));
			}
			else
			{
				var val = TypeTable.ConvertDataToObject((byte)syncData.Type, syncData.FieldData);
				page.Set(field, val);
			}
		}
	}
}
