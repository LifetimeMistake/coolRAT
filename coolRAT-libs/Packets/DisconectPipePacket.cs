using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class DisconnectPipePacket : Packet
    {
        public Guid ClientId;
        public PipeType PipeType;
        public DisconnectPipePacket(Guid clientId, PipeType pipeType)
        {
            ClientId = clientId;
            PipeType = pipeType;
            Type = "DisconnectPipePacket";
        }

        public new static DisconnectPipePacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<DisconnectPipePacket>(packet_string);
        }
    }
}
