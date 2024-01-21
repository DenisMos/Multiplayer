using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml.Linq;
using Assets.Module.Multiplayer.Scripts.Handlers;
using Assets.Module.Multiplayer.Scripts.Scheduler;
using Assets.Multiplayer.Framework;
using Assets.Multiplayer.Scheduler;
using Assets.Multiplayer.Scripts.Protocols.RPC;
using UpdServerCore.Clients;
using UpdServerCore.Framework;

namespace Assets.Multiplayer.Scripts.Framework
{
	internal static class FuncHandleApi
	{
		private static INetworkScheduler _scheduler;
		private static SendAPI SendServer;
		private static bool _initialize = false;

		public static void Initialize(
			INetworkScheduler updInstance,
			SendAPI sender)
		{
			if(_initialize == false)
			{
				_initialize = true;
				_scheduler = updInstance;
				SendServer = sender;
			}
			else
			{
				throw new NetworkInformationException(401);
			}
		}

		private static void ThrowException()
			=> throw new Exception("Udp web socket не установлен.");

		private static void WriteInStream(Stream stream, byte type, byte[] bytes)
		{
			var size_header = BitConverter.GetBytes(bytes.Length);

			stream.WriteByte(type);
			stream.Write(size_header, 0, size_header.Length);
			stream.Write(bytes, 0, bytes.Length);
		}

		private static byte[] Serialize(string name, object[] objects)
		{
			using(var mem = new MemoryStream())
			{
				WriteInStream(mem, 0x10, Encoding.Default.GetBytes(name));

				if(objects != null)
				{
					foreach(var item in objects)
					{
						var type = TypeTable.GetTypeData(item);
						WriteInStream(mem, type, TypeTable.ConvertToData(item));
					}
				}
				return mem.ToArray();
			}
		}

		public static void Post(string name, params object[] args)
		{
			if(_scheduler == null)
				ThrowException();

			var data = Serialize(name, args);

            _scheduler.Post(new QueueItemNet(
                    () => SendServer.SendBytes(() => new RPCProto(data)),
                    RulesSending.Timeout,
                    name
                ));
        }

		public static void Call(RPCProtoData rPCProtoData)
		{
			if(_scheduler.IsServer)
			{
				_scheduler.Post(new QueueItemNet(
				   () => SendServer.SendBytes(() => new RPCProto(rPCProtoData.Data)),
					 RulesSending.Timeout,
				   rPCProtoData.Name
			   ));

				SendServer.Call(rPCProtoData);
			}
			else
			{
                SendServer.Call(rPCProtoData);
            }
		}
	}
}
