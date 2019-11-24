
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.PacketProcessors
{
    public class DisconnectPipePacket_PacketProcessor
    {
        TcpConnection Connection;
        DisconnectPipePacket Packet;
        Dictionary<Guid, Client> ConnectedClients;

        public DisconnectPipePacket_PacketProcessor(TcpConnection connection, DisconnectPipePacket packet, Dictionary<Guid, Client> connectedClients)
        {
            Connection = connection;
            Packet = packet;
            ConnectedClients = connectedClients;
        }

        public bool Process()
        {
            if (!ConnectedClients.ContainsKey(Packet.ClientId))
            {
                PipeDisconnectedPacket result_fail = new PipeDisconnectedPacket(Packet.ClientId, Packet.PipeType, false);
                Connection.SendPacket(result_fail);
                return false;
            }

            switch (Packet.PipeType)
            {
                case PipeType.Main:
                    Console.WriteLine($"Main pipe of client {Packet.ClientId} unbound successfully.");
                    ConnectedClients[Packet.ClientId].Pipes.MainPipe = null;
                    break;
                case PipeType.Ping:
                    Console.WriteLine($"Ping pipe of client {Packet.ClientId} unbound successfully.");
                    ConnectedClients[Packet.ClientId].Pipes.PingPipe = null;
                    break;
                case PipeType.Shell:
                    Console.WriteLine($"Shell pipe of client {Packet.ClientId} unbound successfully.");
                    ConnectedClients[Packet.ClientId].Pipes.ShellPipe = null;
                    break;
            }
            PipeDisconnectedPacket result_success = new PipeDisconnectedPacket(Packet.ClientId, Packet.PipeType, true);
            Connection.SendPacket(result_success);
            return true;
        }
    }
}
