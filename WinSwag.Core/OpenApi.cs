using NSwag;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public static class OpenApi
    {
        public static OpenApiSettings DefaultSettings { get; set; } = OpenApiSettings.Default;

        public static async Task<Response> SendRequestAsync(Operation operation, OpenApiSettings settings = null)
        {
            settings = settings ?? DefaultSettings ?? OpenApiSettings.Default;

            using (var http = new HttpClient())
            {
                var requestUri = new StringBuilder(
                    operation.Document.BaseUrl +
                    operation.Path + '?');

                var request = new HttpRequestMessage
                {
                    Method = operation.Method.ToHttpMethod()
                };

                settings.ConfigureRequest?.Invoke(request);

                foreach (var parameter in operation.Parameters)
                    await parameter.ApplyAsync(request, requestUri);

                // TODO: This is temporary. Operations should reference a subset of the doc's
                // security schemes and only those that are enabled should be included.
                foreach (var securityScheme in operation.Document.SecuritySchemes)
                    await securityScheme.ApplyAsync(request, requestUri);

                requestUri.Length--; // remove trailing '?' or '&'
                var finalUri = requestUri.ToString();
                request.RequestUri = new Uri(finalUri);

                try
                {
                    var response = await http.SendAsync(request);
                    return await Response.FromResponseMessageAsync(finalUri, response, settings);
                }
                catch (HttpRequestException e)
                {
                    return new Response(finalUri, e);
                }
            }
        }
    }

    public class ArgumentTypeInfo
    {
        public Func<SwaggerParameter, bool> IsApplicable { get; set; }
        public Type Type { get; set; }
    }

    public class ResponseContentTypeInfo
    {
        public Func<HttpResponseMessage, bool> IsApplicable { get; set; }
        public Type Type { get; set; }
    }
}
