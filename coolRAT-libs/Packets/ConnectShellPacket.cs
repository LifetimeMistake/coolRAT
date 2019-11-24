using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class ConnectShellPacket : Packet
    {
        public Guid ClientUniqueId;

        public ConnectShellPacket(Guid clientUniqueId)
        {
            Type = "ConnectShellPacket";
            ClientUniqueId = clientUniqueId;
        }

        public static new ConnectShellPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<ConnectShellPacket>(packet_string);
        }
    }
}
