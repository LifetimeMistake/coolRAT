using coolRAT.Libs.Packets;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace coolRAT.Libs
{
    public class TcpConnection
    {
        public int ConnectTimeout;
        public TcpClient Client;

        public TcpConnection()
        {
            ConnectTimeout = 10000;
            Client = new TcpClient();
        }

        public TcpConnection(TcpClient client)
        {
            ConnectTimeout = 10000;
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

        public void SendPacket(Packet packet)
        {
            Write(packet.Serialize());
            Console.WriteLine($"Sent packet of type {packet.Type}");

            if(packet.Type == "Shell_IO_ChangedPacket")
            {
                Shell_IO_ChangedPacket packet__ = packet as Shell_IO_ChangedPacket;
                //Console.WriteLine($"ChangeType: {packet__.ChangeType}; Change: {packet__.Change}");
            }
        }

        public string ReadPacket()
        {
            string json = Read();
            Packet packet = JsonConvert.DeserializeObject<Packet>(json);
            Console.WriteLine($"Received packet of type {packet.Type}");

            if (packet.Type == "Shell_IO_ChangedPacket")
            {
                Shell_IO_ChangedPacket packet__ = Shell_IO_ChangedPacket.Deserialize(json);
                //Console.WriteLine($"ChangeType: {packet__.ChangeType}; Change: {packet__.Change}");
            }
            return json;
        }

        public void WriteJson<T>(object Data)
        {
            Write(JsonConvert.SerializeObject(Data));
        }

        public T ReadJson<T>()
        {
            return JsonConvert.DeserializeObject<T>(Read());
        }

        public void Write(string Data)
        {
            NetworkStream stream = Client.GetStream();
            byte[] bytes = Encoding.UTF8.GetBytes(Data + "\n\n");
            stream.Write(bytes, 0, bytes.Length);
        }

        public string Read()
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
            //Console.WriteLine($"Received: '{received}'");
            return received;
        }

        public static explicit operator TcpConnection(TcpClient c) => new TcpConnection(c);
    }
}