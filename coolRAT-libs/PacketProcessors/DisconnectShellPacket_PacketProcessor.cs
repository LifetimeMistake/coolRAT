using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace coolRAT.Libs.PacketProcessors
{
    public class DisconnectShellPacket_PacketProcessor
    {
        TcpConnection Connection;
        DisconnectShellPacket Packet;
        Dictionary<Guid, Client> ConnectedClients;

        public DisconnectShellPacket_PacketProcessor(TcpConnection connection, DisconnectShellPacket packet, Dictionary<Guid, Client> connectedClients)
        {
            Connection = connection;
            Packet = packet;
            ConnectedClients = connectedClients;
        }

        // Not sure how to approach this

        public bool Process()
        {
            return false;
        }
    }
}
