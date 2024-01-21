using Assets.Multiplayer.Attributes;

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;

namespace Assets.Multiplayer
{
	/// <summary>Сетевой менеджер.</summary>
	public sealed class NetworkManager : MonoBehaviour
	{
		[Header("Настройки сервера")]
		[SerializeField]
		private int Port = 5000;

		[SerializeField]
		private string IP;

		[Header("Настройки маршрута")]
		[SerializeField]
		private int PortDistanation = 5001;

		[SerializeField]
		private string IPDistanation;

		[Header("Автоматически разворачивать сервер")]
		/// <summary>Автоматически запускает сервер при запуске unity в debug.</summary>
		[SerializeField]
		private bool DebugServerStart;

		[Header("Автоматически разворачивать клиент")]
		/// <summary>Автоматически запускает сервер при запуске unity в debug.</summary>
		[SerializeField]
		private bool DebugClientStart;

		[SerializeField]
		private bool ConfigurationFromFile;

		[Header("Сетевой мост")]
		[SerializeField]
		private UnityNetworkAdapter NetworkAdapter;

		private void Start()
		{
			if(NetworkAdapter == null)
			{
				throw new Exception("Сетевой адаптер не выбран.");
			}

			if(ConfigurationFromFile)
			{
				var filename = "netconfig.cfg";
				if(!System.IO.File.Exists(filename))
				{
					using(var fileStream = File.Create(filename))
					{
						var ip = Encoding.Unicode.GetBytes($"{IP}\n");
						var port = Encoding.Unicode.GetBytes($"{Port}\n");
						var portDist = Encoding.Unicode.GetBytes($"{PortDistanation}");

						fileStream.Write(ip, 0, ip.Length);
						fileStream.Write(port, 0, port.Length);
						fileStream.Write(ip, 0, ip.Length);
						fileStream.Write(portDist, 0, portDist.Length);
					}
				}

				using(var fileStream = new StreamReader(filename, Encoding.Unicode))
				{
					IP = fileStream.ReadLine();
					Port = int.Parse(fileStream.ReadLine().TrimEnd('\n'));
					IPDistanation = fileStream.ReadLine();
					PortDistanation = int.Parse(fileStream.ReadLine().TrimEnd('\n'));
				}
			}

			if(DebugServerStart)
			{
#if UNITY_EDITOR
				//Port = 5000;
				StartServer();
#else
				var port = PortDistanation;
				PortDistanation = Port;
				Port = port;

				if(!StartClient())
				{ 
					Application.Quit();
				}
#endif
			}
			else if(DebugClientStart)
			{
#if UNITY_EDITOR
				var port = PortDistanation;
				PortDistanation = Port;
				Port = port;

				if(!StartClient())
				{
					Application.Quit();
				}
#else
				Port = PortDistanation;
				StartServer();
#endif
			}
		}

		private void OnDisable()
		{
			NetworkAdapter.Dispose();
		}

		public void StartServer()
		{
			Debug.Log("Unity начинает инициализацию сервера.");

			NetworkAdapter.StartServer(IP, Port);
		}

		public bool StartClient()
		{
			Debug.Log("Unity начинает инициализацию клиента.");

			return NetworkAdapter.StartClient(IP, Port, IPDistanation, PortDistanation);
		}

		#region UnityMenu

		[ContextMenu("Update network's")]
		public void UpdateNetwork()
		{
			var bh = FindObjectsOfType<MonoBehaviour>(true);

			var bindingFlags = BindingFlags.Instance |
				  BindingFlags.NonPublic |
				  BindingFlags.Public |
				  BindingFlags.DeclaredOnly;

			foreach(var b in bh)
			{
				var type = b.GetType();
				var fields = type.GetFields(bindingFlags).Concat(type.BaseType.GetFields(bindingFlags));

				foreach(var field in fields)
				{
					var attr = field.GetCustomAttribute<NetworkOptionsAttribute>();
					if(attr != null)
					{
						if(attr.NetOpt == NetworkProto.NetworkId)
						{
							field.SetValue(b, Guid.NewGuid().ToString());
						}
					}
				}
			}
		}

		#endregion
	}
}
