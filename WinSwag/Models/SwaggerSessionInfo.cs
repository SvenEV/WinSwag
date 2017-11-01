using System;

namespace WinSwag.Models
{
    public class SwaggerSessionInfo
    {
        public string DisplayName { get; }

        public string Url { get; }

        public Guid Guid { get; }

        public SwaggerSessionInfo(string displayName, string url, Guid guid)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Guid = guid;
        }
    }
}