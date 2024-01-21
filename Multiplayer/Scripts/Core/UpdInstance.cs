using System;
using System.Net;
using System.Linq;
using System.Net.Sockets;

using UnityEngine;

namespace UdpServerCore.Core
{

	public class UpdInstance : INetworkService, IDisposable
	{
		private Socket _udpSocket;
		private EndPoint _endPoint;
		private IAsyncResult _result;
		private byte[] _buffer;

		public EventHandler<ResponseData> EventHandler { get; set; }

		public bool Disposed { get; private set; }

		public int State { get; private set; }

		public string Name { get; }

		public UpdInstance(string name)
		{
			Name = name;
		}

		public void Start(IPAddress iPAddress, int port, int bufferLength)
		{
			if(!_udpSocket.Connected)
			{
				var localIP = new IPEndPoint(iPAddress, port);
				_udpSocket.Bind(localIP);

				Listen(bufferLength);
			}
		}

		public void Listen(int bufferLength)
		{
			_buffer = new byte[bufferLength];
			_endPoint = new IPEndPoint(IPAddress.Any, 0);
			_udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
			_udpSocket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref _endPoint, AsyncResponse, _udpSocket);
		}

		public void StopListen()
		{
			_udpSocket.Close();
		}

		private void AsyncResponse(IAsyncResult ar)
		{
			_result = ar;
			try
			{
				EndPoint endPoint = new IPEndPoint(0, 0);
				int count = _udpSocket.EndReceiveFrom(_result, ref endPoint);

				Debug.Log($"{_buffer[1]} | {endPoint}");

				var content = new ResponseData(_buffer.Take(count).ToArray(), endPoint);

				EventHandler.Invoke(this, content);
			}
			catch(Exception exp)
			{
				Console.WriteLine(exp.Message);
			}
			finally
			{
				if(!Disposed)
				{
					_udpSocket.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref _endPoint, AsyncResponse, null);
				}
            }
		}

		public void SendTo(byte[] data, EndPoint endPoint)
		{
			try
			{
                _udpSocket.SendTo(data, SocketFlags.None, endPoint);
                Debug.Log($"Send data {data[1]} | {endPoint}");
            }
			catch
			{
				throw;
			}
		}

		public void Initialize()
		{
			if(_udpSocket == null)
			{
				_udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			}
		}

		public void Stop()
		{
			_udpSocket?.Close();
		}

		public void Dispose()
		{
			if(!Disposed)
			{
				Stop();
				_udpSocket?.Dispose();
				Disposed = true;
			}
		}
	}
}
