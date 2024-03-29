﻿using System.Net;

namespace UpdServerCore.Core
{
    public sealed class ResponseData
    {
        public byte[] Data { get; }

        public EndPoint EndPoint { get; }

        public ResponseData(byte[] data, EndPoint endPoint) 
        { 
            Data= data;
            EndPoint = endPoint;
        }
    }
}
