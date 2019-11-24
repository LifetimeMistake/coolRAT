using System;
using System.Net.Sockets;

namespace coolRAT.Libs
{
    public class Client
    {
        public Guid UniqueId;
        public TcpPipes Pipes;

        public Client(TcpPipes pipes)
        {
            UniqueId = Guid.NewGuid();
            Pipes = pipes;
        }

        public Client(Guid uniqueId, TcpPipes pipes)
        {
            UniqueId = uniqueId;
            Pipes = pipes;
        }

        public Client(string uniqueId, TcpPipes pipes)
        {
            UniqueId = new Guid(uniqueId);
            Pipes = pipes;
        }
    }
}