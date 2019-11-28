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
        static void Main(string[] args)
        {
            Dictionary<Guid, Client> ConnectedClients = new Dictionary<Guid, Client>();
            CAP_Server server = new CAP_Server(8888);
            server.OnClientConnected += (s, e) =>
            {
                ConnectedClients.Add(e.Client.UniqueId, e.Client);
            };
            server.Start();
            Console.WriteLine("Client Authorization Protocol server version 1.0.0.0 started");
            Application.Run();
        }
    }
}
