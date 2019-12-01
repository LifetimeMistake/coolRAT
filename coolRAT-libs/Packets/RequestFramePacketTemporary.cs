using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    class RequestFramePacketTemporary : Packet
    {
        [JsonConstructor]
        public RequestFramePacketTemporary(Guid clientId) : base(clientId)
        {
            Type = "RequestFramePacketTemporary";
        }

        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static RequestFramePacketTemporary Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<RequestFramePacketTemporary>(packet_string);
        }
    }
}
