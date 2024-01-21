using System;
using System.IO;
using System.Net;
using UdpServerCore.Core;

namespace UdpServerCore.Protocols.Sync
{
    public sealed class SyncProtocol : IProtocol
    {
        public byte Proto => 0x10;

        /// <summary>Токен синхронизации.</summary>
        private Guid TokenSync { get; }

        /// <summary>Идентификатор поля.</summary>
        private long FieldId { get; }

        /// <summary>Данные.</summary>
        private byte[] FieldData { get; }

        private byte Type { get; }

        public SyncProtocol(Guid guid, long id, byte type, byte[] data)
        { 
            TokenSync = guid;
            FieldId   = id;
            FieldData = data;
            Type = type;
        }

        public void SendTo(INetworkService updInstance, EndPoint endPoint)
        {
            if(updInstance.Disposed) return;

            try
            {
                var buffers = GetBuffers();

                updInstance.SendTo(buffers, endPoint);
            }
            catch 
            {
                throw;
            }
        }

        public bool CheckBuffer(byte[] bytes)
        {
            if(bytes.Length < 30)
            {
                return false;
            }

            return true;
        }

        public static bool TryParse(byte[] buffers, out ISyncData syncData)
        {
            try
            {
                var proto = buffers[0];

                if(proto != 0x10)
                {
                    syncData = null;
                    return false;
                }

                byte[] tokenBuffers = new byte[16];
                Array.Copy(buffers,1,tokenBuffers,0,16);

                byte[] typeBuffers = new byte[8];
                Array.Copy(buffers, 17, typeBuffers, 0, 8);

                var typeByte = buffers[25];

                byte[] lenBuffer = new byte[4];
                Array.Copy(buffers, 26, lenBuffer, 0, 4);

                byte[] body = new byte[buffers.Length - 30];
                Array.Copy(buffers, 30, body, 0, body.Length);

                syncData = new SyncData(proto, tokenBuffers, typeBuffers, typeByte, body);
                return true;
            }
            catch
            {
                syncData = null;
                return false;
            }
        }

        public byte[] GetBuffers()
        {
            var header = TokenSync.ToByteArray();                //16B
            var fieldId = BitConverter.GetBytes(FieldId);        //8B
            var type = Type;                                     //1B
            var length = BitConverter.GetBytes(FieldData.Length);//4B
            var data = FieldData;                                //N

            var buffer = new byte[30 + data.Length];

            using(var mem = new MemoryStream(buffer))
            {
                mem.WriteByte(Proto);
                mem.Write(header, 0, 16);
                mem.Write(fieldId, 0, 8);
                mem.WriteByte(type);
                mem.Write(length, 0, 4);
                mem.Write(data, 0, data.Length);

                return mem.ToArray();
            }
        }
    }
}
