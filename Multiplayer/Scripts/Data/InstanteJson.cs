using System;
using System.IO;
using System.Runtime.Serialization.Json;
using UnityEngine;

namespace Assets.Multiplayer.Data
{
    [Serializable]
    public sealed class InstanteJson
    {
        [SerializeField]
        private short ChannelId => 150;

        [SerializeField]
        public Vector3 Vector { get; }

        [SerializeField]
        public Quaternion Quaternion { get; }

        [SerializeField]
        public string Path { get; }

        public InstanteJson(string path, Vector3 vector, Quaternion quaternion)
        {
            Vector = vector;
            Quaternion = quaternion;
            Path = path;
        }


        public byte[] Serialize() 
        {
            var js = new DataContractJsonSerializer(this.GetType());
            using(var mem = new MemoryStream()) 
            {
                js.WriteObject(mem, this);
                return mem.ToArray();
            }
        }
    }
}
