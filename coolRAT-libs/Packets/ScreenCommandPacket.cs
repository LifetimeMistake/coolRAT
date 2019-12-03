using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public enum CommandType
    {
        RequestFrame, // Arguments: Full, Partial
        SetState // Arguments: Running, Stopped
    }
    public class ScreenCommandPacket : Packet
    {
        public Guid UniqueScreenId;
        public CommandType CommandType;
        public string Arguments;
        public ScreenCommandPacket(Guid uniqueClientId, Guid uniqueScreenId, CommandType commandType, string arguments = "") : base(uniqueClientId)
        {
            CommandType = commandType;
            UniqueScreenId = uniqueScreenId;
            Arguments = arguments;
            Type = "ScreenCommandPacket";
        }

        public new string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public new static ScreenCommandPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<ScreenCommandPacket>(packet_string);
        }
    }
}
