using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class Packet
    {
        public Guid UniquePacketId = Guid.NewGuid();
        public Guid UniqueClientId;
        public string Type = "Packet";
        public DateTime TimeStamp = DateTime.Now;
        public Packet(Guid uniqueClientId)
        {
            UniqueClientId = uniqueClientId;
            Type = "Packet";
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Packet Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<Packet>(packet_string);
        }
    }
}
