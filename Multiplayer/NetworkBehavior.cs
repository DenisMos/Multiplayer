using System;

using UnityEngine;

using Assets.Multiplayer.Attributes;
using Assets.Multiplayer.Scripts;
using Assets.Multiplayer.Scripts.Framework;

namespace Assets.Multiplayer
{
	/// <summary>Обработчик синхронизации.</summary>
	public class NetworkBehavior : MonoBehaviour
	{
		[NetworkOptions(NetworkProto.NetworkId)]
		[SerializeField]
		private string NetworkId;

		private void OnValidate()
		{
			if(string.IsNullOrEmpty(NetworkId))
			{
				NetworkId = Guid.NewGuid().ToString();
			}
		}

		public Guid Id => new Guid(NetworkId);

		[RPCMethods(NetworkMethodsProto.RPC)]
		/// <summary>Создаёт объект для всех в сети.</summary>
		/// <param name="model"></param>
		/// <param name="position"></param>
		/// <param name="quaternion"></param>
		public static void InstantiateInNetwork(GameObject model, Vector3 position, Quaternion quaternion)
		{
			
		}

		public void Call()
		{
			FuncHandleApi.Post("Test", 0);
		}
	}
}
