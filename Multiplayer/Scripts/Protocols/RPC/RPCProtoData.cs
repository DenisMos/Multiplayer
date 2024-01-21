using System.Net;

namespace Assets.Multiplayer.Scripts.Protocols.RPC
{
	public sealed class RPCProtoData
	{
		public string Name { get; set; }

		public object[] Args { get; set; }

		public byte[] Data { get; set; }

		public EndPoint Sender { get; set; }

		public RPCProtoData(byte[] data)
		{
			Data = data;
			var type = data[0];
		}
	}
}
