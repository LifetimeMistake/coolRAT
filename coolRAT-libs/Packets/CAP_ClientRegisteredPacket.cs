using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class CAP_ClientRegisteredPacket : Packet
    {
        public CAP_ClientRegisteredPacket(Guid registeredClientId) : base(registeredClientId)
        {
            Type = "CAP_ClientRegisteredPacket";
        }

        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static CAP_ClientRegisteredPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<CAP_ClientRegisteredPacket>(packet_string);
        }
    }
}
