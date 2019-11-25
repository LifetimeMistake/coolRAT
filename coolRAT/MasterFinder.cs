using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using coolRAT.Libs;
using coolRAT.Libs.Packets;

namespace coolRAT.Slave
{
    public struct MasterServerInfo
    {
        public Client LocalClient;
        public IPEndPoint RemoteServer;
        public TcpConnection MainConnection;
        public bool HasValues;

        public MasterServerInfo(Client localClient, IPEndPoint remoteServer, TcpConnection mainConnection) : this()
        {
            LocalClient = localClient;
            RemoteServer = remoteServer;
            MainConnection = mainConnection;
            HasValues = true;
        }
    }
    public static class MasterFinder
    {
        private static ConnectionSettings Settings;

        public static void RunInit(ConnectionSettings settings)
        {
            Settings = settings;
        }
        public static MasterServerInfo ConnectToMasterServer()
        {
            if (Settings.IsEmpty())
                throw new ArgumentNullException(nameof(Settings));
            SlaveGlobalData.FindLocalEndPoint();
            Console.Write("[MasterFinder] Attempting to find the master server... ");
            string local_ip_range = string.Join(".", SlaveGlobalData.LocalEndpoint.Address.ToString().Split('.').Reverse().Skip(1).Reverse().ToArray());
            for (int i = 1; i<256; i++)
            {
                try
                {
                    
                    TcpConnection conn = new TcpConnection();
                    conn.ConnectTimeout = 1000;
                    if(!conn.Connect($"{local_ip_range}.{i}", Settings.Port)) continue;

                    // Authenticate the connection
                    AuthenticateConnectionPacket authpacket = new AuthenticateConnectionPacket(Auth.ClientAuthenticationTicket);
                    conn.SendPacket(authpacket);
                    ConnectionAuthenticatedPacket response = ConnectionAuthenticatedPacket.Deserialize(conn.ReadPacket());
                    if (response.ConnectionAuthenticated)
                    {
                        Console.WriteLine("Success");
                        Client client = new Client(response.UniqueClientId, new TcpPipes(conn, null, null));
                        MasterServerInfo inf = new MasterServerInfo(client, conn.Client.Client.RemoteEndPoint as IPEndPoint, conn);
                        return inf;
                    }
                    
                }
                catch(Exception e) { Console.WriteLine(e.Message); }
            }
            Console.WriteLine("Fail");
            return new MasterServerInfo();
        }
    }
}
