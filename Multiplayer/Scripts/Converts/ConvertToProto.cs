using System;
using UdpServerCore.Framework;
using UdpServerCore.Protocols.Sync;

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
