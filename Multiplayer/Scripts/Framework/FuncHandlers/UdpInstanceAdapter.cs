﻿using Assets.Module.Multiplayer.Scripts.Handlers;
using Assets.Multiplayer.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UpdServerCore.Clients;
using UpdServerCore.Core;

namespace Assets.Multiplayer.Scripts.Framework
{
    internal class UdpInstanceAdapter
    {
        private List<string> MethodsList = new List<string>();

        public UdpInstanceAdapter(INetworkScheduler scheduler, SendAPI sendAPI)
        {
            FuncHandleApi.Initialize(scheduler, sendAPI);
        }
    }
}
