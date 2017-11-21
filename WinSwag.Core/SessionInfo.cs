using System;

namespace WinSwag.Core
{
    public class SessionInfo
    {
        public string DisplayName { get; }

        public string Url { get; }

        public Guid Guid { get; }

        public SessionInfo(string displayName, string url, Guid guid)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Guid = guid;
        }
    }
}