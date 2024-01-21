using Assets.Multiplayer.Attributes;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Multiplayer.Scripts.Containers
{
	public sealed class MethodsContainer
	{
		private Dictionary<RPCMethodsAttribute, MethodInfo> _parameters;

		public int Count => _parameters.Count;

		public MethodsContainer() 
		{
			_parameters = new Dictionary<RPCMethodsAttribute, MethodInfo>();
		}

		public void Add(RPCMethodsAttribute rPCMethodsAttribute, MethodInfo methodInfo)
		{ 
			_parameters.Add(rPCMethodsAttribute, methodInfo);
		}
	}
}
