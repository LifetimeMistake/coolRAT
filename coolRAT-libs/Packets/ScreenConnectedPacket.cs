using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class ScreenConnectedPacket : Packet
    {
        public Guid UniqueScreenId;
        public bool Success;
        public ScreenConnectedPacket(Guid uniqueClientId, Guid uniqueScreenId, bool success) : base(uniqueClientId)
        {
            UniqueScreenId = uniqueScreenId;
            Success = success;
            Type = "ScreenConnectedPacket";
        }

        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static ScreenConnectedPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<ScreenConnectedPacket>(packet_string);
        }
    }
}
