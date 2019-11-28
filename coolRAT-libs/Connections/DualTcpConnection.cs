using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace coolRAT.Libs.Connections
{
    public class PacketReceivedEventArgs : EventArgs
    {
        Packet Packet;

        public PacketReceivedEventArgs(Packet packet)
        {
            Packet = packet;
        }
    }
    public class DualTcpConnection
    {
        public TcpConnection IncomingConnection;
        public TcpConnection OutgoingConnection;

        public delegate void PacketReceivedHandler(object sender, PacketReceivedEventArgs e);
        public Dictionary<string, PacketReceivedHandler> PacketReceiverSubscriptions;
        public List<Packet> UnhandledPackets;
        public Thread PacketSenderTh;
        public Thread PacketReceiverTh;

        public void SenderLoop()
        {
            while(true)
            {
                while (OutgoingConnection == null) Thread.Sleep(50);
                while (!OutgoingConnection.Client.Connected) Thread.Sleep(50);
                OutgoingConnection.WaitReadyMessage();
                while (OutgoingConnection.PacketQueue.Count == 0) Thread.Sleep(10);
                OutgoingConnection.SendPacket(OutgoingConnection.PacketQueue[0]);
                OutgoingConnection.PacketQueue.RemoveAt(0);
            }
        }

        public void ReceiverLoop()
        {
            while(true)
            {
                while (IncomingConnection == null) Thread.Sleep(50);
                while (!IncomingConnection.Client.Connected) Thread.Sleep(50);
                IncomingConnection.SendReadyMessage();
                string packet_raw = IncomingConnection.ReceivePacket();
                Packet packet = Packet.Deserialize(packet_raw);
                if(!PacketReceiverSubscriptions.ContainsKey(packet.Type))
                {
                    // No packet handler connected for this type of packet.
                    // Add the packet to the unhandled packets list to be possibly handled by another handler in the future.
                    UnhandledPackets.Add(packet);
                    continue;
                }

                if(PacketReceiverSubscriptions[packet.Type] == null)
                {
                    // No packet handler connected for this type of packet.
                    // Add the packet to the unhandled packets list to be possibly handled by another handler in the future.
                    PacketReceiverSubscriptions.Remove(packet.Type);
                    UnhandledPackets.Add(packet);
                    continue;
                }

                PacketReceiverSubscriptions[packet.Type].Invoke(this, new PacketReceivedEventArgs(packet));
            }
        }

        public void StartSender()
        {
            if (PacketSenderTh != null) if (PacketSenderTh.IsAlive) StopSender();
            PacketSenderTh = new Thread(new ThreadStart(SenderLoop));
            PacketSenderTh.IsBackground = true;
            PacketSenderTh.Start();
        }

        public void StopSender()
        {
            if (PacketSenderTh == null) return;
            if (PacketSenderTh.IsAlive) PacketSenderTh.Abort();
            PacketSenderTh = null;
        }

        public void StartReceiver()
        {
            if (PacketReceiverTh != null) if (PacketReceiverTh.IsAlive) StopReceiver();
            PacketReceiverTh = new Thread(new ThreadStart(ReceiverLoop));
            PacketReceiverTh.IsBackground = true;
            PacketReceiverTh.Start();
        }

        public void StopReceiver()
        {
            if (PacketReceiverTh == null) return;
            if (PacketReceiverTh.IsAlive) PacketReceiverTh.Abort();
            PacketReceiverTh = null;
        }

        public void StartAll()
        {
            StartSender();
            StartReceiver();
        }

        public void StopAll()
        {
            StopSender();
            StopReceiver();
        }


        public DualTcpConnection(TcpConnection incomingConnection, TcpConnection outgoingConnection, Dictionary<string, PacketReceivedHandler> packetReceiverSubscriptions)
        {
            IncomingConnection = incomingConnection;
            OutgoingConnection = outgoingConnection;
            PacketReceiverSubscriptions = packetReceiverSubscriptions;
            UnhandledPackets = new List<Packet>();
        }

        public DualTcpConnection()
        {
            PacketReceiverSubscriptions = new Dictionary<string, PacketReceivedHandler>();
            UnhandledPackets = new List<Packet>();
        }
    }
}
