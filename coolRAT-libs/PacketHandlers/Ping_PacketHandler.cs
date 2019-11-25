using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.PacketHandlers
{
    public class Ping_PacketHandler : PacketHandler
    {
        public Ping_PacketHandler(TcpConnection connection) : base(connection) { }

        public override void HandlePacket(object sender, string packet_raw)
        {
            throw new NotImplementedException();
        }
    }
}
