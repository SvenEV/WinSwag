using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace WinSwag.Core
{
    public class SessionInfo
    {
        private readonly Lazy<string> _urlHash;

        public string DisplayName { get; }

        public string Url { get; }

        [JsonIgnore]
        public string UrlHash => _urlHash.Value;

        public SessionInfo(string displayName, string url)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            _urlHash = new Lazy<string>(() => MD5Hash(Url));
        }

        private static string MD5Hash(string data)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
                var result = new StringBuilder(bytes.Length * 2);

                for (var i = 0; i < bytes.Length; i++)
                    result.Append(bytes[i].ToString("X2"));

                return result.ToString();
            }
        }
    }
}