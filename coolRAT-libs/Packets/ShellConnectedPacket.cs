using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class ShellConnectedPacket : Packet
    {
        public Guid ShellUniqueId;
        public bool Success;
        public ShellConnectedPacket(Guid clientUniqueId, Guid shellUniqueId, bool success) : base(clientUniqueId)
        {
            Type = "ShellConnectedPacket";
            ShellUniqueId = shellUniqueId;
            Success = success;
        }

        public static new ShellConnectedPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<ShellConnectedPacket>(packet_string);
        }
    }
}
