using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Connections
{
    public class DualTcpConnection
    {
        public TcpConnection IncomingConnection;
        public TcpConnection OutgoingConnection;

        public DualTcpConnection()
        {
            IncomingConnection = new TcpConnection();
            OutgoingConnection = new TcpConnection();
        }
    }
}
