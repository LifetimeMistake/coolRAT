using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class ConnectShellPacket : Packet
    {
        public ConnectShellPacket(Guid clientUniqueId) : base(clientUniqueId)
        {
            Type = "ConnectShellPacket";
        }

        public static new ConnectShellPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<ConnectShellPacket>(packet_string);
        }
    }
}
