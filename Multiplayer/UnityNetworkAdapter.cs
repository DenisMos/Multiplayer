using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;

using UnityEngine;

using UdpServerCore.Core;
using UdpServerCore.Clients;
using UdpServerCore.Servers;
using UdpServerCore.Protocols.RPE;
using UdpServerCore.Framework.ClientList;

using Assets.Multiplayer.Framework;
using Assets.Multiplayer;
using Assets.Multiplayer.Scheduler;
using Assets.Multiplayer.Scripts.Containers;
using Assets.Multiplayer.Scripts.Framework;

using Multiplayer.Scripts.Handlers;
using Multiplayer.Scripts.Services.Auth;
using Multiplayer.Scripts.Services.Sync;
using Multiplayer;

/// <summary>������� ������� ������� ��� Unity.</summary>
public class UnityNetworkAdapter : MonoBehaviour, IDisposable, INetworkAdapter
{
	private bool _disposed;
	private INetworkService _updInstance;
	private SyncHandler _syncServers;
	private SyncFieldSend _syncFieldSend;
	private SynchronizationContext _synchronizationContext;
	public EndPointList ClientTable { get; private set; }

	public INetworkScheduler NetworkScheduler { get; private set; }

	private MethodsContainer MethodsContainer { get; } = new MethodsContainer();

	public SyncMainContainer Containers { get; set; }

	private SendAPI SendAPI { get; set; }

	[Header("�������������� ��� 'behaviours'")]
	[SerializeField]
	private bool _findAllBehaviour;

	[Header("�������������� Transform")]
	[SerializeField]
	private bool _findProtoTransform;

	[Header("����� Debug")]
	[SerializeField]
	private bool _isDebug;

	private UdpInstanceAdapter _udpInstanceAdapter;
	private AuthService _authService;
	private SyncService _syncService;
	private string IP;
	private int Port;
	private bool IsClient;

	private void SetLoggerSinks()
	{
		SinkLogAdapter.SetSink(x => Debug.Log(x));
		SinkLogAdapter.SetSinkError(x => Debug.LogError(x));
	}

	private void Awake()
	{
		SetLoggerSinks();

		Containers = new SyncMainContainer();
		RegistrationAllBehaviours();
		_synchronizationContext = SynchronizationContext.Current.CreateCopy();

		ClientTable = new EndPointList();
		_updInstance = new UpdInstance("new");

		NetworkScheduler = new NetworkScheduler(
			_synchronizationContext, 
			ClientTable, 
			_updInstance);

		_authService = new AuthService(_updInstance, NetworkScheduler);
		_syncService = new SyncService(NetworkScheduler, Containers);

		_syncServers = new SyncHandler(
			_updInstance,
			Containers,
			_authService,
			_syncService,
			_isDebug);

		SendAPI = new SendAPI(_updInstance, ClientTable, _syncServers);
		_udpInstanceAdapter = new UdpInstanceAdapter(NetworkScheduler, SendAPI);
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

		_syncFieldSend = new SyncFieldSend(NetworkScheduler, ClientTable, Containers, _updInstance);
		_syncFieldSend.Status = true;
		_syncFieldSend.StartSending();

		NetworkScheduler.IsServer = true;

		if(_isDebug)
		{
			NetworkLogger.Log($"Server started: '{ip}:{port}'");
		}
	}

	public bool StartClient(string ip, int port, string ipDistance, int portDistance)
	{
		_syncServers.StartClient(port);

		if(_isDebug)
		{
			NetworkLogger.Log($"Client started: '{ip}:{port}'");
		}

		NetworkScheduler.IsServer = false;

		IP = ipDistance;
		Port = portDistance;

		var s = _authService.TryConnect(ipDistance, portDistance);

		_syncFieldSend = new SyncFieldSend(NetworkScheduler, ClientTable, Containers, _updInstance);
		_syncFieldSend.Status = true;
		_syncFieldSend.StartSending();

		return s;
	}

	public void Dispose()
	{
		if(!_disposed)
		{
			if(!NetworkScheduler.IsServer)
			{
				var startProto = new RPEProtocol(RPECommandData.Exit);
				startProto.SendTo(_updInstance, new IPEndPoint(IPAddress.Parse(IP), Port));
			}

			NetworkScheduler.Close();
			_syncFieldSend?.Stop();
			_updInstance.Stop();
			_updInstance.Dispose();

			_disposed = true;
		}
	}
}
