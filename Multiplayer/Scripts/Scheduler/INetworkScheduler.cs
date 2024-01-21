using System;
using System.Collections.Generic;
using System.Net;
using UdpServerCore.Core;
using UdpServerCore.Framework;
using UdpServerCore.Protocols;

namespace Assets.Multiplayer.Scheduler
{
	/// <summary>Сетевой планировщик.</summary>
	public interface INetworkScheduler : IDisposable
	{
        public bool IsServer { get; set; }

        /// <summary>Возвращает все разрешённые данные.</summary>
        /// <returns></returns>
        public IEnumerable<IPageData<FieldData>> GetAllowFields();

		/// <summary>Вызывает изменения в планировщике.</summary>
		//public void Post(Func<byte[]> func, FieldData field, EndPoint endPoint);

        public void Post(byte[] data);

        public void Post(QueueItemNet queueItemNet);


        public void Resolve();

		public bool Check(EndPoint endPoint);

		public bool TryConnect(EndPoint endPoint);

		public bool TryDisconnect(EndPoint endPoint);
	}
}
