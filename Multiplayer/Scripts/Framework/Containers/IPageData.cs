using Assets.Multiplayer.Attributes;

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace UdpServerCore.Framework
{
	public interface IPageData<TFields>
	{
		Guid Token { get; }

		List<TFields> Fields { get; }
	}

	public class FieldData
	{
		public long Id { get; }

		public FieldInfo Value { get; set; }

		public byte Type { get; }

		public NetworkProto NetOptEnums { get; }

		public FieldData(long id, byte type, FieldInfo value, NetworkProto netOptEnums) 
		{ 
			Id = id;
			Type = type;
			Value = value;
			NetOptEnums = netOptEnums;
		}
	}
}
