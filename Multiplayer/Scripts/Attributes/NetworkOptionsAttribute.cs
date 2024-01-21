using System;

namespace Assets.Multiplayer.Attributes
{
	internal class NetworkOptionsAttribute : Attribute
	{
		public NetworkProto NetOpt { get; }

		public long Id { get; set; }

		public long? UserId { get; set; }

		public NetworkOptionsAttribute(NetworkProto netOptEnums)
		{
			NetOpt = netOptEnums;
		}

		public NetworkOptionsAttribute(NetworkProto netOptEnums, long userId)
		{
			NetOpt = netOptEnums;
			UserId = userId;
		}
	}

	public class RPCMethodsAttribute : Attribute
	{
		public NetworkMethodsProto NetOpt { get; }

		public Guid Token { get; private set; }

		public string Name { get; private set; }

		public bool IsStatic { get; private set; }

		public void Set(Guid guid, string name, bool isStatic)
		{
			Name = name;
			IsStatic = isStatic;

			if(!IsStatic)
			{ 
				Token = guid;
			}
		}

		public RPCMethodsAttribute(NetworkMethodsProto netOptEnums)
		{
			NetOpt = netOptEnums;
		}
	}
}
