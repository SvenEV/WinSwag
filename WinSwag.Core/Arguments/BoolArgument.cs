using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class BoolArgument : ArgumentBase
    {
        private bool _value;

        public bool Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public override object ObjectValue
        {
            get => Value;
            set => Value = (bool)value;
        }

        public override bool HasValue => true; // TODO: Make value of type bool? and serialize only if not null

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri) =>
            StringArgument.ApplyAsync(Parameter.Specification, _value.ToString(), request, requestUri, ContentType);

        public override JToken GetSerializedValue() => JToken.FromObject(Value);

        public override Task SetSerializedValueAsync(JToken o) => Task.FromResult(Value = o.ToObject<bool>());
    }
}
