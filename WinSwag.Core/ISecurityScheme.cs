using NSwag;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public interface ISecurityScheme
    {
        SwaggerSecurityScheme Specification { get; }

        string Scheme { get; }

        string Name { get; }

        string Description { get; }

        bool HasDescription { get; }

        /// <summary>
        /// Applies the security scheme to the HTTP request body or request URI
        /// (depending on the type of the scheme) and sets the necessary headers.
        /// </summary>
        Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri);
    }
}
