using NSwag;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class SecuritySchemeBasic : SecurityScheme
    {
        private string _username;
        private string _password;

        public override string Scheme => "Basic";

        public string Username
        {
            get => _username;
            set => Set(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public SecuritySchemeBasic(string name, SwaggerSecurityScheme specification) : base(name, specification)
        {
            if (specification.Type != SwaggerSecuritySchemeType.Basic)
                throw new ArgumentException($"Scheme type must be '{SwaggerSecuritySchemeType.Basic}'", nameof(specification));
        }

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri)
        {
            var authString = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Username}:{Password}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authString);
            return Task.CompletedTask;
        }
    }
}
