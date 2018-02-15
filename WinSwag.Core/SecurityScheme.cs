using NSwag;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public abstract class SecurityScheme : ObservableObjectEx, ISecurityScheme
    {
        public SwaggerSecurityScheme Specification { get; }

        public abstract string Scheme { get; }

        public string Name { get; }

        public string Description => Specification.Description;

        public bool HasDescription => !string.IsNullOrWhiteSpace(Specification.Description);

        public SecurityScheme(string name, SwaggerSecurityScheme specification)
        {
            Name = name;
            Specification = specification;
        }

        public static ISecurityScheme FromSpec(string name, SwaggerSecurityScheme specification)
        {
            switch (specification.Type)
            {
                case SwaggerSecuritySchemeType.Basic: return new SecuritySchemeBasic(name, specification);
                case SwaggerSecuritySchemeType.ApiKey: return new SecuritySchemeApiKey(name, specification);
                case SwaggerSecuritySchemeType.OAuth2: return new SecuritySchemeOAuth2(name, specification);
                default: throw new NotImplementedException();
            }
        }

        public abstract Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri);
    }
}
