﻿using UnityEngine;

namespace Assets.Multiplayer.Scripts
{
	public static class NetworkStaticAdapter
	{
		private static INetworkAdapter _networkAdapter;

		static NetworkStaticAdapter()
		{
			_networkAdapter = Object.FindObjectOfType<UnityNetworkAdapter>();

			Debug.Log($"ok {_networkAdapter != null}");
		}
	}
}
