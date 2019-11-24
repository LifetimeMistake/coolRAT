using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.PacketProcessors
{
    public class AuthenticateConnectionPacket_PacketProcessor
    {
        TcpConnection Connection;
        AuthenticateConnectionPacket Packet;

        public AuthenticateConnectionPacket_PacketProcessor(TcpConnection connection, AuthenticateConnectionPacket packet)
        {
            Connection = connection;
            Packet = packet;
        }

        public Client Process()
        {
            if(Packet.AuthenticationTicket == Auth.ClientAuthenticationTicket)
            {
                Console.WriteLine("Connection has been accepted.");
                Client client = new Client(new TcpPipes(Connection, null, null));
                ConnectionAuthenticatedPacket authenticated_packet = new ConnectionAuthenticatedPacket(true, client.UniqueId);
                Connection.SendPacket(authenticated_packet);
                return client;
            }
            else
            {
                Console.WriteLine("Connection has been denied.");
                ConnectionAuthenticatedPacket authenticated_packet = new ConnectionAuthenticatedPacket(false, Guid.Empty);
                Connection.SendPacket(authenticated_packet);
                return null;
            }
        }
    }
}
