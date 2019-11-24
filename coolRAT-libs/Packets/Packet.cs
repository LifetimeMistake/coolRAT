using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class Packet
    {
        public Guid UniqueId = Guid.NewGuid();
        public string Type;
        public DateTime TimeStamp = DateTime.Now;
        public Packet()
        {
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
