using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdServerCore.Protocols.Sync
{
    public sealed class SyncData : ISyncData
    {
        public Guid TokenSync { get; private set; }

        public long FieldId { get; private set; }

        public byte[] FieldData { get; private set; }

        public int Type { get; private set; }

        public int Proto { get; }

        public SyncData(byte proto, byte[] token, byte[] fieldId, byte td, byte[] data)
        { 
            Proto = proto;
            TokenSync = new Guid(token);
            FieldId = BitConverter.ToInt64(fieldId, 0);
            FieldData = data;
            Type = td;
        }
    }
}
