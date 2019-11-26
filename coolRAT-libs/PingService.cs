using coolRAT.Libs.PacketHandlers;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace coolRAT.Libs
{
    public struct ClientInfo
    {
        public Guid ClientUniqueId;
        public Client Client;
        public PacketListenerLoop ListenerLoop;
        public DateTime LastPingReceived;
        public int TimeoutValue;

        public ClientInfo(Guid clientUniqueId, Client client, PacketListenerLoop listenerLoop, int timeoutValue = 10000)
        {
            ClientUniqueId = clientUniqueId;
            Client = client;
            ListenerLoop = listenerLoop;
            LastPingReceived = DateTime.Now;
            TimeoutValue = timeoutValue;
        }

        public void WaitForTimeout()
        {
            while((DateTime.Now - LastPingReceived).TotalMilliseconds < TimeoutValue)
            {
                Thread.Sleep(200);
            }
        }
    }

    public class PingService
    {
        public Dictionary<Guid, ClientInfo> ConnectedClients;
        public bool AbortLoop;

        public PingService(Dictionary<Guid, Client> connectedClients)
        {
            ConnectedClients = new Dictionary<Guid, ClientInfo>();
            foreach (KeyValuePair<Guid, Client> kvp in connectedClients)
                ConnectedClients.Add(kvp.Key, new ClientInfo(kvp.Key, kvp.Value, null));
        }

        private void Loop()
        {
            while(!AbortLoop)
            {
                foreach (KeyValuePair<Guid, ClientInfo> kvp in ConnectedClients)
                    if ((DateTime.Now - kvp.Value.LastPingReceived).TotalMilliseconds < kvp.Value.TimeoutValue)
                        RemoveClient(kvp.Key);
                Thread.Sleep(1000);
            }
            AbortLoop = false;
        }

        public void Start()
        {
            Task.Run(() => Loop());
        }

        public void Stop()
        {
            AbortLoop = true;
        }

        public void RunActive()
        {
            foreach(KeyValuePair<Guid, ClientInfo> kvp in ConnectedClients)
            {
                ClientInfo inf = ConnectedClients[kvp.Key];
                inf.ListenerLoop = new PacketListenerLoop(kvp.Value.Client.Pipes.PingPipe,
                    new Ping_PacketHandler(kvp.Value.Client.Pipes.PingPipe, kvp.Value));
                Task.Run(() => inf.ListenerLoop.Run());
                ConnectedClients[kvp.Key] = inf;
                Task.Run(() => ConnectedClients[kvp.Key].ListenerLoop.Run());
                Task.Run(() =>
                {
                    PingPacket pingPacket = new PingPacket(Guid.NewGuid());
                    kvp.Value.Client.Pipes.PingPipe.SendPacket(pingPacket);
                });
            }
            // Send first packet
        }

        public bool AddClient(Client client, bool activeMode)
        {
            if (ConnectedClients.ContainsKey(client.UniqueId)) return false;
            ClientInfo inf = new ClientInfo(client.UniqueId, client, null);
            inf.ListenerLoop = new PacketListenerLoop(client.Pipes.PingPipe, new Ping_PacketHandler(client.Pipes.PingPipe, inf));
            Task.Run(() => inf.ListenerLoop.Run());
            ConnectedClients.Add(client.UniqueId, inf);
            if(activeMode)
            {
                Task.Run(() =>
                {
                    PingPacket pingPacket = new PingPacket(Guid.NewGuid());
                    client.Pipes.PingPipe.SendPacket(pingPacket);
                });
            }
            return true;
        }

        public bool RemoveClient(Guid client)
        {
            if (!ConnectedClients.ContainsKey(client)) return false;
            // Stop the service
            ClientInfo inf = ConnectedClients[client];
            if(inf.ListenerLoop != null)
            {
                inf.ListenerLoop.AbortLoop = true;
                inf.ListenerLoop = null;
            }
            ConnectedClients.Remove(client);
            return true;
        }

        public void RunPassive()
        {
            foreach (KeyValuePair<Guid, ClientInfo> kvp in ConnectedClients)
            {
                ClientInfo inf = ConnectedClients[kvp.Key];
                inf.ListenerLoop = new PacketListenerLoop(kvp.Value.Client.Pipes.PingPipe,
                    new Ping_PacketHandler(kvp.Value.Client.Pipes.PingPipe, inf));
                ConnectedClients[kvp.Key] = inf;
                Task.Run(() => ConnectedClients[kvp.Key].ListenerLoop.Run());
            }
        }
    }
}
