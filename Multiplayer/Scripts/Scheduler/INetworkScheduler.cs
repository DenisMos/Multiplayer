using System;
using System.Collections.Generic;
using System.Net;
using UpdServerCore.Core;
using UpdServerCore.Framework;

namespace Assets.Multiplayer.Scheduler
{
	/// <summary>Сетевой планировщик.</summary>
	public interface INetworkScheduler : IDisposable
	{
		/// <summary>Возвращает все разрешённые данные.</summary>
		/// <returns></returns>
		public IEnumerable<IPageData<FieldData>> GetAllowFields();

		/// <summary>Вызывает изменения в планировщике.</summary>
		public void Post(Func<byte[]> func, FieldData field, EndPoint endPoint);

        public void Post(byte[] data);

        public void Resolve(EndPoint ignorableUser = null, EndPoint target = null);

		public bool Check(EndPoint endPoint);

		public bool TryConnect(EndPoint endPoint);

		public bool TryDisconnect(EndPoint endPoint);
	}
}
