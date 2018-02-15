using NSwag;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class SecuritySchemeApiKey : SecurityScheme
    {
        private string _apiKey;

        public override string Scheme => "API Key";

        public string ApiKey
        {
            get => _apiKey;
            set => Set(ref _apiKey, value);
        }

        public SecuritySchemeApiKey(string name, SwaggerSecurityScheme specification) : base(name, specification)
        {
            if (specification.Type != SwaggerSecuritySchemeType.ApiKey)
                throw new ArgumentException($"Scheme type must be '{SwaggerSecuritySchemeType.ApiKey}'", nameof(specification));
        }

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri)
        {
            if (string.IsNullOrEmpty(_apiKey))
                return Task.CompletedTask;

            switch (Specification.In)
            {
                case SwaggerSecurityApiKeyLocation.Header:
                    request.Headers.Add(Specification.Name, ApiKey);
                    break;

                case SwaggerSecurityApiKeyLocation.Query:
                    requestUri.Append($"{Specification.Name}={Uri.EscapeDataString(ApiKey)}&");
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
