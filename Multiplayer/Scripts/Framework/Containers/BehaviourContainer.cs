using Assets.Multiplayer.Attributes;
using Assets.Multiplayer.Scripts.Containers;
using Assets.Multiplayer.Serilizator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UdpServerCore.Framework;

namespace Assets.Multiplayer.Framework
{
	public sealed class BehaviourContainer : IPageData<FieldData>
	{
		/// <summary>Токен индентификациии.</summary>
		public Guid Token { get; private set; }

		/// <summary>Поля синхронизации.</summary>
		public List<FieldData> Fields { get; } = new List<FieldData>();

		public MethodsContainer MethodsContainer { get; } = new MethodsContainer();

		/// <summary>Ссылка на компонент.</summary>
		public MonoBehaviour NetworkBehaviour { get; }

		public BehaviourContainer(MonoBehaviour networkBehaviour)
		{
			NetworkBehaviour = networkBehaviour;
		}

		/// <summary>Устанавливает поля синхрониации.</summary>
		public void GetFields()
		{
			var bindingFlags = BindingFlags.Instance |
				  BindingFlags.NonPublic |
				  BindingFlags.Public |
				  BindingFlags.DeclaredOnly;

			var type = NetworkBehaviour.GetType();
			var fields = type.GetFields(bindingFlags).Concat(type.BaseType.GetFields(bindingFlags));

			int id = 0;
			foreach(var field in fields)
			{
				var attr = field.GetCustomAttribute<NetworkOptionsAttribute>();
				if(attr != null)
				{
					if(attr.NetOpt == NetworkProto.NetworkId)
					{
						Token = Guid.Parse((string)field.GetValue(NetworkBehaviour));
					}
					else
					{
						attr.Id = id;
						var type_c = TypeTable.GetTypeData(field);

						Fields.Add(new FieldData(id++, type_c, field, NetworkProto.Sync));
					}
				}
			}
		}

		public void GetMethodsAndFunction()
		{
			var type = NetworkBehaviour.GetType();

			var methodInfos = type
				.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
				.Concat(type.BaseType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static));

			foreach(var meth in methodInfos)
			{
				var attr = meth.GetCustomAttribute<RPCMethodsAttribute>();
				if(attr != null)
				{
					attr.Set(Token, meth.Name, meth.IsStatic);
					MethodsContainer.Add(attr, meth);
				}
			}
		}

		/// <summary>Устанавливает синхрониацию трансформации.</summary>
		public void GetTransform()
		{
			var type = NetworkBehaviour.GetType();
			var attrs = type.GetCustomAttributes<NetworkOptionsAttribute>().ToList();

			if(attrs != null && attrs.Count > 0)
			{
				foreach(var atr in attrs)
				{
					if(atr.NetOpt == NetworkProto.Transform)
					{
						var type_c = TypeTable.TransformTypeByte;

						Fields.Add(new FieldData(-5, type_c, null, NetworkProto.Transform));
					}
				}
			}
		}

		public void Set(FieldData field, object val)
		{
			if(field != null)
			{
				field.Value.SetValue(NetworkBehaviour, val);
			}
		}

		public void SetTransform(TransformSerialize transformSerialize)
		{
			if(NetworkBehaviour != null)
			{
				NetworkBehaviour.transform.position   = transformSerialize.Position;
				NetworkBehaviour.transform.rotation   = transformSerialize.Rotation;
				NetworkBehaviour.transform.localScale = transformSerialize.Scale;
			}
		}
	}
}
