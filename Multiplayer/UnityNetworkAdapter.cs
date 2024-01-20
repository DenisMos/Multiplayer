using System;
using System.Net;

using UnityEngine;

using UpdServerCore.Clients;
using UpdServerCore.Core;
using UpdServerCore.Protocols.RPE;
using UpdServerCore.Servers;

using Assets.Multiplayer.Framework;
using System.Threading;
using Assets.Multiplayer;
using System.Collections.Generic;
using Assets.Multiplayer.Scheduler;
using Assets.Multiplayer.Scripts.Containers;
using Assets.Multiplayer.Scripts.Framework;

/// <summary>Сетевой адаптер сокетов для Unity.</summary>
public class UnityNetworkAdapter : MonoBehaviour, IDisposable, INetworkAdapter
{
	private bool _disposed;
	private IUpdInstance _updInstance;
	private SyncServers _syncServers;
	private SyncFieldSend _syncFieldSend;
	private SynchronizationContext _synchronizationContext;
	public INetworkScheduler _networkScheduler;

	private MethodsContainer MethodsContainer { get; } = new MethodsContainer();

    public SyncMainContainer Containers { get; set; }


	[Header("Регистрировать все 'behaviours'")]
	[SerializeField]
	private bool _findAllBehaviour;

	[Header("Синхронизовать Transform")]
	[SerializeField]
	private bool _findProtoTransform;

	[Header("Режим Debug")]
	[SerializeField]
	private bool _isDebug;

	private UdpInstanceAdapter _udpInstanceAdapter;

    private string IP;
	private int Port;
	private bool IsClient;

	private void Awake()
	{
		Containers = new SyncMainContainer();
		RegistrationAllBehaviours();
		_synchronizationContext = SynchronizationContext.Current.CreateCopy();

		_updInstance = new UpdInstance("new");
		_networkScheduler = new NetworkScheduler(!IsClient, Containers, _synchronizationContext, _updInstance);
		_syncServers = new SyncServers(
			_updInstance,
			_synchronizationContext,
			Containers,
			_networkScheduler,
			_isDebug);

		_udpInstanceAdapter = new UdpInstanceAdapter(_networkScheduler);
    }

	private void RegistrationAllBehaviours()
	{
		var behaviours = GetAllObjectOnScene();
		Containers.Add(behaviours);
	}

	private List<BehaviourContainer> GetAllObjectOnScene()
	{
		var list = new List<BehaviourContainer>();
		IEnumerable<MonoBehaviour> behaviours;

		if(!_findAllBehaviour)
		{
			behaviours = FindObjectsOfType<NetworkBehavior>(true);
		}
		else
		{
			behaviours = FindObjectsOfType<MonoBehaviour>(true);
		}

		foreach(var behaviour in behaviours)
		{
			var bh_container = new BehaviourContainer(behaviour);
			bh_container.GetFields();
			bh_container.GetMethodsAndFunction();
			if(_findProtoTransform)
			{
				bh_container.GetTransform();
			}
			list.Add(bh_container);
		}

		return list;
	}

	public void StartServer(string ip, int port)
	{
		_syncServers.StartServer(ip, port);

		_syncFieldSend = new SyncFieldSend(_networkScheduler);
		_syncFieldSend.Status = true;
		_syncFieldSend.StartSending();

		if(_isDebug)
		{
			Debug.Log($"Server started on port: '{port}'");
		}
	}

	public bool StartClient(string ip, int port, string ipDistance, int portDistance)
	{
        _syncServers.StartClient(port);

		if(_isDebug)
		{
			Debug.Log("Client started");
		}

		IsClient = true;

        IP = ipDistance;
		Port = portDistance;

		var s = _syncServers.TryConnect(ipDistance, portDistance);

		_syncFieldSend = new SyncFieldSend(_networkScheduler);
		_syncFieldSend.Status = true;
		_syncFieldSend.StartSending();

		return s;
	}

	public void Dispose()
	{
		if(!_disposed)
		{
			if(IsClient)
			{
				var startProto = new RPEProtocol(RPECommandData.Exit);
				startProto.SendTo((UpdInstance)_updInstance, new IPEndPoint(IPAddress.Parse(IP), Port));
			}

			_syncFieldSend?.Stop();
			_updInstance.Stop();
			_updInstance.Dispose();

			_disposed = true;
		}
	}
}
