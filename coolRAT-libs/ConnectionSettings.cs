using System;
using System.Net;

namespace coolRAT.Libs
{
    public struct ConnectionSettings
    {
        public int Port { get; private set; }
        private bool isEmpty;

        public ConnectionSettings(int port)
        {
            Port = port;
            isEmpty = false;
        }

        public bool IsEmpty()
        {
            return isEmpty;
        }
    }
}