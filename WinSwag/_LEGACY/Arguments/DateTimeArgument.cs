using GalaSoft.MvvmLight;
using Newtonsoft.Json.Linq;
using NSwag;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core.Arguments
{
    public class DateTimeArgument : ObservableObject, ISwaggerArgument<DateTimeOffset?>
    {
        private DateTimeOffset? _value;

        public DateTimeOffset? Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public bool HasValue => _value != null;

        public Task ApplyAsync(SwaggerParameter parameter, HttpRequestMessage request, StringBuilder requestUri, string contentType) =>
            StringArgument.ApplyAsync(parameter, _value?.ToString(), request, requestUri, contentType);

        public JToken GetSerializedValue() => Value == null ? null : JToken.FromObject(Value);

        public Task SetSerializedValueAsync(JToken o) => Task.FromResult(Value = o?.ToObject<DateTimeOffset?>());
    }
}
