using Assets.Multiplayer.Scripts.Scheduler;
using System;
using System.Collections.Generic;

namespace Assets.Multiplayer.Scheduler
{
    public sealed class QueueNetModification
    {
        private Dictionary<Guid ,NetModification> _observerComponents 
            = new Dictionary<Guid, NetModification>();

        public NetField[] GetAllModification()
        {
            var list = new List<NetField>();
            foreach(var item in _observerComponents)
            { 
                var key = item.Key;
                var value = item.Value;

                value.GetNetFields(list);
            }

            return list.ToArray();
        }

        public void SetModification(NetField netField)
        {
            var token = netField.Token;

            if(_observerComponents.ContainsKey(token))
            {
                _observerComponents[token].Set(netField);
            }
            else
            {
                var component = new NetModification(token);
                component.Set(netField);
                _observerComponents.Add(token, component);
            }
        }
    }
}
