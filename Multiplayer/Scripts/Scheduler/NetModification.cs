using Assets.Multiplayer.Attributes;
using System;
using System.Collections.Generic;
using UdpServerCore.Framework;

namespace Assets.Multiplayer.Scripts.Scheduler
{
    public sealed class NetModification
    {
        private Guid _guid;
        private Dictionary<long, NetField> _net;

        public void Set(NetField field)
        {
            var id = field.FieldData.Id;
            if(!_net.ContainsKey(id))
            {
                field.WasChanged = true;
                _net[id] = field;
            }
            else
            { 
                _net[id].WasChanged = true;
            }
        }

        public void GetNetFields(List<NetField> fields)
        {
            foreach(var item in _net)
            { 
                var value = item.Value;

                if(value.WasChanged)
                {
                    fields.Add(value);
                }
            }
        }

        public NetModification(
            Guid guid) 
        {
            _guid = guid;

            _net = new Dictionary<long, NetField>();
        }

        public NetModification(
            Guid guid,
            NetField netField)
        {
            _guid = guid;

            _net = new Dictionary<long, NetField>()
            { 
                { netField.FieldData.Id, netField }
            };
        }
    }

    public sealed class NetField
    { 
        public Guid Token { get; }
        public FieldData FieldData { get; }
        public bool WasChanged { get; set; }

        public NetField(
            Guid token,
            FieldData data)
        {
            Token = token;
            FieldData = data;
        }
    }
}
