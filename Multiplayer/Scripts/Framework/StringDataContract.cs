using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdServerCore.Framework
{

    internal class StringDataContract : IDataContract<string>
    {
        public byte Id { get; }

        public string Value { get; }

        public int Type => 3;

        public byte[] Data { get; }

        public StringDataContract(byte id, string @value)
        {
            Id = id;
            Value = @value;

            var typeData = (byte)Type;

            var values = Encoding.Default.GetBytes(Value);

            using(var mem = new MemoryStream(1024))
            {
                //Write type byte
                mem.WriteByte(typeData);
                //Write channel
                mem.WriteByte(Id);
                //Write value
                mem.Write(values, 0, values.Length);

                Data = mem.ToArray();
            }
        }

        public StringDataContract(byte[] data)
        {
            Data = data;
            Id = data[1];
            Value = Encoding.Default.GetString(Data, 2, Data.Length - 2);
        }
    }
}
