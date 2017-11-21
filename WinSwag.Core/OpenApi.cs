using NSwag;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public static class OpenApi
    {
        public static OpenApiSettings DefaultSettings { get; set; } = OpenApiSettings.Default;

        public static async Task<Response> SendRequestAsync(Operation operation, string baseUrl, OpenApiSettings settings = null)
        {
            settings = settings ?? DefaultSettings ?? OpenApiSettings.Default;

            using (var http = new HttpClient())
            {
                var requestUri = new StringBuilder(baseUrl + operation.Specification.Path + '?');

                var request = new HttpRequestMessage
                {
                    Method = operation.Specification.Method.ToHttpMethod(),
                };

                settings.ConfigureRequest?.Invoke(request);
                // UserAgent = { new ProductInfoHeaderValue("WinSwag", ApplicationInstance.Current.Services.GetService<ApplicationInfo>().Version) }

                foreach (var parameter in operation.Parameters)
                    await parameter.ApplyAsync(request, requestUri);

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


    public class OpenApiSettings
    {
        public IList<ArgumentTypeInfo> ArgumentTypes { get; }

        public IList<ResponseContentTypeInfo> ResponseContentTypes { get; }

        public Type FallbackArgumentType { get; set; }

        public Type FallbackResponseContentType { get; set; }

        public Action<HttpRequestMessage> ConfigureRequest { get; set; }

        public OpenApiSettings()
        {
            ArgumentTypes = new List<ArgumentTypeInfo>();
            ResponseContentTypes = new List<ResponseContentTypeInfo>();
        }

        public OpenApiSettings(OpenApiSettings settings)
        {
            ArgumentTypes = new List<ArgumentTypeInfo>(settings.ArgumentTypes);
            ResponseContentTypes = new List<ResponseContentTypeInfo>(settings.ResponseContentTypes);
            FallbackArgumentType = settings.FallbackArgumentType;
            FallbackResponseContentType = settings.FallbackResponseContentType;
            ConfigureRequest = settings.ConfigureRequest;
        }

        public static readonly OpenApiSettings Default = new OpenApiSettings
        {
            FallbackArgumentType = typeof(StringArgument),
            FallbackResponseContentType = typeof(StringResponse),

            ArgumentTypes =
            {
                new ArgumentTypeInfo
                {
                    Type = typeof(BoolArgument),
                    IsApplicable = spec => spec.Type == NJsonSchema.JsonObjectType.Boolean,
                },
                new ArgumentTypeInfo
                {
                    Type = typeof(DateTimeArgument),
                    IsApplicable = spec => spec.Format == "date-time"
                },
                new ArgumentTypeInfo
                {
                    Type = typeof(EnumArgument),
                    IsApplicable = spec => spec.ActualSchema.IsEnumeration
                },
            },

            ResponseContentTypes =
            {
                new ResponseContentTypeInfo
                {
                    Type = typeof(JsonResponse),
                    IsApplicable = msg => msg.Content?.Headers.ContentType?.MediaType == "application/json"
                }
            }
        };
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
