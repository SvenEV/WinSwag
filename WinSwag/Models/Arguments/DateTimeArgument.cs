using Newtonsoft.Json.Linq;
using NSwag;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Models.Arguments
{
    public class DateTimeArgument : SwaggerArgument
    {
        private DateTimeOffset? _value;

        public DateTimeOffset? Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public DateTimeArgument(SwaggerParameter parameter) : base(parameter)
        {
        }

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri) =>
            StringArgument.ApplyAsync(Parameter, _value?.ToString(), request, requestUri);


        public override JToken GetSerializedValue() => Value == null ? null : JToken.FromObject(Value);

        public override Task SetSerializedValueAsync(JToken o) => Task.FromResult(Value = o?.ToObject<DateTimeOffset?>());
    }
}
