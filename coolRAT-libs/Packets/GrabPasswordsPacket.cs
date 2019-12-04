using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class GrabPasswordsPacket : Packet
    {
        public GrabPasswordsPacket(Guid uniqueClientId) : base(uniqueClientId)
        {
            Type = "GrabPasswordsPacket";
        }

        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static GrabPasswordsPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<GrabPasswordsPacket>(packet_string);
        }
    }
}
