using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public enum ShellState
    {
        Running,
        Stopped
    }
    public class SetShellStatePacket : Packet
    {
        public Guid ClientUniqueId;
        public Guid ShellUniqueId;
        public ShellState ShellState;

        public SetShellStatePacket(Guid clientUniqueId, Guid shellUniqueId, ShellState shellState)
        {
            ClientUniqueId = clientUniqueId;
            ShellUniqueId = shellUniqueId;
            ShellState = shellState;
            Type = "SetShellStatePacket";
        }

        public static new SetShellStatePacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<SetShellStatePacket>(packet_string);
        }
    }
}
