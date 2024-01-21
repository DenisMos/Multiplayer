using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Module.Multiplayer.Scripts.Scheduler
{
    public enum RulesSending : byte
    {
        None = 0x0,
        Cycle = 0xC1,
        Timeout = 0xB1,
    }
}
