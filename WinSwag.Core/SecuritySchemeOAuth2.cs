using NSwag;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class SecuritySchemeOAuth2 : SecurityScheme
    {
        public override string Scheme => "OAuth 2";

        public IReadOnlyList<OAuth2Scope> Scopes { get; }

        public SecuritySchemeOAuth2(string name, SwaggerSecurityScheme specification) : base(name, specification)
        {
            if (specification.Type != SwaggerSecuritySchemeType.OAuth2)
                throw new ArgumentException($"Scheme type must be '{SwaggerSecuritySchemeType.OAuth2}'", nameof(specification));

            Scopes = specification.Scopes?
                .Select(scope => new OAuth2Scope(scope.Key, scope.Value))
                .ToImmutableList() ?? ImmutableList<OAuth2Scope>.Empty;
        }

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri)
        {
            throw new NotImplementedException();
        }

    }

    public class OAuth2Scope : ObservableObjectEx
    {
        private bool _isEnabled;

        public string Name { get; }

        public string Description { get; }

        public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

        public bool IsEnabled
        {
            get => _isEnabled;
            set => Set(ref _isEnabled, value);
        }

        public OAuth2Scope(string name, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
        }
    }
}
