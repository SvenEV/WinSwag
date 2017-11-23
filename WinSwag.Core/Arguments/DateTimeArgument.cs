using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class DateTimeArgument : ArgumentBase
    {
        private DateTimeOffset? _value;

        public DateTimeOffset? Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public override object ObjectValue
        {
            get => Value;
            set => Value = (DateTimeOffset?)value;
        }

        public override object InitialValue => null;

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri) =>
            StringArgument.ApplyAsync(Parameter, _value?.ToString(), request, requestUri, ContentType);

        public override JToken GetSerializedValue() => ObjectValue == null ? null : JToken.FromObject(ObjectValue);

        public override Task SetSerializedValueAsync(JToken o) => Task.FromResult(ObjectValue = o?.ToObject<DateTimeOffset?>());
    }
}
