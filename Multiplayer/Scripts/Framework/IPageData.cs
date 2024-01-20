using Assets.Multiplayer.Attributes;

using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace UpdServerCore.Framework
{
	public interface IPageData<TFields>
	{
		Guid Token { get; }

		List<TFields> Fields { get; }

		void Set(long id, object obj, object val);
	}

	public sealed class PageData : IPageData<FieldData>
	{
		public Guid Token { get; }

		public List<FieldData> Fields { get; set; }

		public PageData(Guid token, int capacity)
		{
			Token = token;
			Fields = new List<FieldData>(capacity);
		}

		public void Set(long id, object obj, object val)
		{
			var da = Fields.FirstOrDefault(x=>x.Id == id);

			if(da != null)
			{ 
				da.Value.SetValue(obj, val);
			}
		}
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
