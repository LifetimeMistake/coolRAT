using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace coolRAT.Libs.PacketHandlers
{
    public class Ping_PacketHandler : PacketHandler
    {
        public ClientInfo ClientInfo;
        public Ping_PacketHandler(TcpConnection connection, ClientInfo client) : base(connection) { ClientInfo = client; }

        public override void HandlePacket(object sender, string packet_raw)
        {
            Packet packet = Packet.Deserialize(packet_raw);
            if(packet.Type == "PingPacket")
            {
                // Handle incoming ping
                PingPacket received_ping = PingPacket.Deserialize(packet_raw);
                Thread.Sleep(1000);
                PingPacket new_pingpacket = new PingPacket(received_ping.Checksum);
                Connection.SendPacket(new_pingpacket);
            }
        }
    }
}
