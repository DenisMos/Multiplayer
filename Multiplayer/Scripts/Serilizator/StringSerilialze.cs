using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdServerCore.Framework;

namespace UpdServerCore.Serilizator
{

    public static class Serilialze
    {
        public static IDataContract<string> SerializeData(long id, string data)
        {
            var serialize = new StringDataContract((byte)id, data);

            return serialize;
        }

        public static IDataContract DeSerializeData(byte[] data)
        {
            switch(data[0])
            {
                case 0x03:
                    return new StringDataContract(data);
            }

            return null;
        }
    }
}
