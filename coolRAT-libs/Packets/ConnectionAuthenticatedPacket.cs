using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class ConnectionAuthenticatedPacket : Packet
    {
        public bool ConnectionAuthenticated;
        public Guid UniqueClientId; // Unique Id to assign to the client

        public ConnectionAuthenticatedPacket(bool connectionAuthenticated, Guid uniqueClientId)
        {
            Type = "ConnectionAuthenticatedPacket";
            ConnectionAuthenticated = connectionAuthenticated;
            UniqueClientId = uniqueClientId;
        }

        public new static ConnectionAuthenticatedPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<ConnectionAuthenticatedPacket>(packet_string);
        }
    }
}
