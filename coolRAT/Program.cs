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

namespace coolRAT.Slave
{
    class Program
    {
        static void Main(string[] args)
        {
            CAP_Client client = new CAP_Client(new IPEndPoint(IPAddress.Parse("192.168.5.6"), 8888));
            Console.WriteLine("Client Authorization Protocol client initialized");
            Console.WriteLine("Registering client...");
            Guid clientId = client.RegisterClient();
            if(clientId == Guid.Empty)
            {
                Console.WriteLine("Failed to register the client.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Registered the client successfully. Authorization ticket: {clientId}");
            Console.WriteLine("Connecting...");
            Client c = client.CreateClient(clientId);
            if(c == null)
            {
                Console.WriteLine("Failed to connect");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Connected!");
            Console.ReadLine();
            return;
        }
    }
}
