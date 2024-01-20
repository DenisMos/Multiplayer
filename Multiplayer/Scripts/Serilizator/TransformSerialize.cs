using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Multiplayer.Serilizator
{
    public sealed class TransformSerialize
    {
        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public Vector3 Scale { get; set; }

        public static byte[] Serialize(Vector3 position, Quaternion quaternion, Vector3 scale) 
        {
            var pos = position;
            var rot = quaternion;
            var sca = scale;

            using(var mem = new MemoryStream())
            {
                mem.Write(BitConverter.GetBytes(pos.x), 0, 4);
                mem.Write(BitConverter.GetBytes(pos.y), 0, 4);
                mem.Write(BitConverter.GetBytes(pos.z), 0, 4);
                mem.Write(BitConverter.GetBytes(rot.x), 0, 4);
                mem.Write(BitConverter.GetBytes(rot.y), 0, 4);
                mem.Write(BitConverter.GetBytes(rot.z), 0, 4);
                mem.Write(BitConverter.GetBytes(rot.w), 0, 4);
                mem.Write(BitConverter.GetBytes(sca.x), 0, 4);
                mem.Write(BitConverter.GetBytes(sca.y), 0, 4);
                mem.Write(BitConverter.GetBytes(sca.z), 0, 4);

                return mem.ToArray();
            }
        }

        public bool Diff(TransformSerialize transformSerialize)
        {
            if((this.Position - transformSerialize.Position).magnitude > 0)
            {
                return true;
            }
            if((!Quaternion.Equals(this.Rotation, transformSerialize.Rotation)))
            {
                return true;
            }
            if((this.Scale - transformSerialize.Scale).magnitude > 0)
            {
                return true;
            }
            return false;
        }

        public static TransformSerialize Deserialize(byte[] bytes)
        {
            byte[] buffer = new byte[40];

            using(var mem = new MemoryStream(bytes))
            {
                mem.Read(buffer, 0, 12);
                var xpos = BitConverter.ToSingle(mem.ToArray(), 0);
                var ypos = BitConverter.ToSingle(mem.ToArray(), 4);
                var zpos = BitConverter.ToSingle(mem.ToArray(), 8);
                var pos = new Vector3(xpos, ypos, zpos);

                var xrot = BitConverter.ToSingle(mem.ToArray(), 12);
                var yrot = BitConverter.ToSingle(mem.ToArray(), 16);
                var zrot = BitConverter.ToSingle(mem.ToArray(), 20);
                var wrot = BitConverter.ToSingle(mem.ToArray(), 24);
                var quat = new Quaternion(xrot, yrot, zrot, wrot);

                var xs = BitConverter.ToSingle(mem.ToArray(), 28);
                var ys = BitConverter.ToSingle(mem.ToArray(), 32);
                var zs = BitConverter.ToSingle(mem.ToArray(), 36);
                var sc = new Vector3(xs, ys, zs);

                var ser = new TransformSerialize()
                { 
                    Position = pos,
                    Rotation = quat,
                    Scale = sc,
                };

                return ser;
            }
        }
    }
}
