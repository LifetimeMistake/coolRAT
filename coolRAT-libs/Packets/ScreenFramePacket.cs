using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public enum FrameType
    {
        Full,
        Partial
    }
    public class ScreenFramePacket : Packet
    {
        [JsonIgnore]
        public Bitmap Frame;
        public FrameType FrameType;
        public Size FrameSize;
        public string FrameSerialized;

        public ScreenFramePacket(Guid uniqueClientId, FrameType frameType, Bitmap frame) : base(uniqueClientId)
        {
            Frame = frame;
            FrameType = frameType;
            FrameSize = frame.Size;
            Type = "ScreenFramePacket";
        }
        [JsonConstructor]
        public ScreenFramePacket(Guid uniqueClientId, FrameType frameType, Size size, string frameSerialized) : base(uniqueClientId)
        {
            Frame = null;
            FrameType = frameType;
            FrameSize = size;
            FrameSerialized = frameSerialized;
        }

        public new string Serialize()
        {
            byte[] byte_array = ImageUtils.ToByteArray(Frame);
            FrameSerialized = ByteSerializer.Serialize(byte_array);
            return JsonConvert.SerializeObject(this);
        }

        public static new ScreenFramePacket Deserialize(string packet_raw)
        {
            ScreenFramePacket framepacket = JsonConvert.DeserializeObject<ScreenFramePacket>(packet_raw);
            Image frame = ImageUtils.FromByteArray(ByteSerializer.Deserialize(framepacket.FrameSerialized));
            framepacket.FrameSerialized = null;
            framepacket.Frame = (Bitmap)frame;
            return framepacket;
        }
    }
}
