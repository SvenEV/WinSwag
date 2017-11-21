using GalaSoft.MvvmLight;
using Newtonsoft.Json.Linq;
using NSwag;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core.Arguments
{
    public class StringArgument : ObservableObject, ISwaggerArgument<string>
    {
        public string Value { get; set; }

        public bool HasValue => !string.IsNullOrEmpty(Value);

        public Task ApplyAsync(SwaggerParameter parameter, HttpRequestMessage request, StringBuilder requestUri, string contentType) =>
            ApplyAsync(parameter, Value, request, requestUri, contentType);

        public static Task ApplyAsync(SwaggerParameter parameter, string value, HttpRequestMessage request, StringBuilder requestUri, string contentType)
        {
            if (string.IsNullOrEmpty(value))
                return Task.CompletedTask;

            switch (parameter.Kind)
            {
                case SwaggerParameterKind.Path:
                    requestUri = requestUri.Replace("{" + parameter.Name + "}", value ?? "");
                    break;

                case SwaggerParameterKind.Header:
                    request.Headers.Add(parameter.Name, value);
                    break;

                case SwaggerParameterKind.Query:
                    requestUri.Append($"{parameter.Name}={Uri.EscapeDataString(value)}&");
                    break;

                case SwaggerParameterKind.Body:
                    request.Content = new StringContent(value, Encoding.UTF8);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    break;

                case SwaggerParameterKind.FormData:
                    var formData = new MultipartFormDataContent();
                    formData.Add(new StringContent(value), parameter.Name);
                    request.Content = formData;
                    break;

                case SwaggerParameterKind.ModelBinding:
                    throw new NotImplementedException(); // TODO
            }

            return Task.CompletedTask;
        }

        public JToken GetSerializedValue() => Value == null ? null : JToken.FromObject(Value);

        public Task SetSerializedValueAsync(JToken o) => Task.FromResult(Value = o?.ToObject<string>());
    }
}
