using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdServerCore.Protocols.RPE
{
    public enum RPECommandData : byte
    {
        Error = 0x00,
        None = 0x01,
        Subsribe = 0xAA,
        Stop = 0xBB,
        Start = 0xCC,
        Alive = 0xDD,
        Exit = 0xFF,
    }
}
