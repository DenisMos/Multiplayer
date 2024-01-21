using Assets.Multiplayer.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using UdpServerCore.Core;
using UdpServerCore.Protocols;
using UdpServerCore.Protocols.RPE;

namespace Assets.Multiplayer.Scripts.Protocols.RPC
{
	internal class RPCProto : IProtocol
	{
		public byte Proto => 0x2;

		public static bool Check(byte[] bytes) => bytes[0] == 0x2;

		private readonly byte[] _data;

		public RPCProto(byte[] data)
		{ 
			_data = data;
		}

		public bool CheckBuffer(byte[] bytes)
		{
			return bytes[0] == Proto;
		}

		public byte[] GetBuffers()
		{
			using(var mem = new MemoryStream())
			{
				mem.WriteByte(Proto);
				mem.Write(_data, 0, _data.Length);

				return mem.ToArray();
			}
		}

		public static bool TryParse(byte[] bytes, out RPCProtoData protoData)
		{
			protoData = null;
			if(bytes[0] != 0x2)
			{
				return false;
			}

			try
			{
				var type = bytes[1];
				var name_len = BitConverter.ToInt32(bytes, 2);
				var name = Encoding.Default.GetString(bytes, 6, (int)name_len);

				var pointer = 6 + (int)name_len;

				var list = new List<object>();
				while(pointer < bytes.Length)
				{
					var ty = bytes[pointer++];
					var len = BitConverter.ToInt32(bytes, pointer);
					pointer += 4;

					byte[] des_data = new byte[len];
					Array.Copy(bytes,pointer, des_data, 0, len);
					var data = TypeTable.ConvertDataToObject(ty, des_data);
					list.Add(data);
					pointer += len;
				}
				protoData = new RPCProtoData(bytes)
				{
					Name = name,
					Args = list.ToArray(),
				};

				return true;
			}
			catch
			{
				return false;
			}
		}

		public void SendTo(INetworkService udpInstance, EndPoint endPoint)
		{
			try
			{
				var buffers = GetBuffers();
				udpInstance.SendTo(buffers, endPoint);
			}
			catch
			{
				throw;
			}
		}
	}
}
