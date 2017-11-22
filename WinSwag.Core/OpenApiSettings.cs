using System;
using System.Collections.Generic;
using System.Net.Http;

namespace WinSwag.Core
{
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
                }
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
}
