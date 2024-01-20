using System;
using UpdServerCore.Framework;
using UpdServerCore.Protocols.Sync;

namespace Assets.Multiplayer.Scripts.Converts
{
    public static class ConvertToProto
    {
        public static SyncProtocol ToProto(this FieldData field, Guid token, byte[] data)
        {
            return new SyncProtocol(token, field.Id, field.Type, data);
        }
    }
}
