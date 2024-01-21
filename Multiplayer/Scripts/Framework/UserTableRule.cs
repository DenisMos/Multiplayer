using Assets.Multiplayer.Framework;
using System.Collections.Generic;
using System.Net;

namespace Assets.Module.Multiplayer.Scripts.Framework
{
    public class UserTableRule
    {
        public Dictionary<string, EndPoint> Rules { get; } 
            = new Dictionary<string, EndPoint>();

        public UserTableRule() 
        { 
        
        }


    }
}
