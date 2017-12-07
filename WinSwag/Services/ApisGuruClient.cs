using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WinSwag.Services
{
    public class ApisGuruClient
    {
        private const string _url = "https://api.apis.guru/v2/list.json";
        private readonly Lazy<Task> _initTask;
        private Dictionary<string, ApiInfo> _apis;

        public ApisGuruClient()
        {
            _initTask = new Lazy<Task>(Init);
        }

        private async Task Init()
        {
            var http = new HttpClient();
            try
            {
                var json = await http.GetStringAsync(_url);
                await Task.Run(() => _apis = JsonConvert.DeserializeObject<Dictionary<string, ApiInfo>>(json));
            }
            catch
            {
                _apis = new Dictionary<string, ApiInfo>();
            }
        }

        public async Task<IReadOnlyList<ApiInfo>> QueryAsync(string queryText, CancellationToken cancellationToken)
        {
            await _initTask.Value;

            if (cancellationToken.IsCancellationRequested)
                return null;

            if (string.IsNullOrWhiteSpace(queryText))
                return Array.Empty<ApiInfo>();

            var text = Normalize(queryText);

            return _apis
                .Where(api =>
                    Normalize(api.Key).Contains(text) ||
                    Normalize(api.Value.PreferredVersion?.Info.Title).Contains(text) ||
                    Normalize(api.Value.PreferredVersion?.SwaggerUrl).Contains(text))
                .Select(api => api.Value)
                .Take(8)
                .OrderBy(api => api.PreferredVersion?.Info.Title)
                .ToList();

            string Normalize(string s) => s == null ? "" : Regex.Replace(s, "[^a-zA-Z0-9äöüß\\-]", "").Trim().ToLower();
        }

    }
    public class ApiInfo
    {
        [JsonProperty("preferred")]
        private string _preferredVersion;

        public ApiVersion PreferredVersion => _preferredVersion == null
            ? Versions.FirstOrDefault().Value
            : Versions.TryGetValue(_preferredVersion, out var v) ? v : null;

        public IReadOnlyDictionary<string, ApiVersion> Versions { get; set; }
    }

    public class ApiVersion
    {
        public ApiVersionInfo Info { get; set; }
        public string SwaggerUrl { get; set; }
    }

    public class ApiVersionInfo
    {
        public string Title { get; set; }
        public string Description { get; set; }

        [JsonProperty("x-logo")]
        public ApiVersionInfoLogo Logo { get; set; }
    }

    public class ApiVersionInfoLogo
    {
        public string Url { get; set; }
    }
}
