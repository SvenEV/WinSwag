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

        // A bool argument is "default" if
        // (a) a default bool value is specified and the selected value equals that default value -or-
        // (b) a default bool value is NOT specified and the selected value is 'false'
        public override bool HasNonDefaultValue =>
            _value != (Parameter.Specification.Default is bool b ? b : false);

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri) =>
            StringArgument.ApplyAsync(Parameter.Specification, _value.ToString(), request, requestUri, ContentType);

        public override JToken GetSerializedValue() => JToken.FromObject(Value);

        public override Task SetSerializedValueAsync(JToken o)
        {
            var defaultBool = Parameter.Specification.Default is bool b ? b : false;
            Value = o?.ToObject<bool>() ?? defaultBool;
            return Task.CompletedTask;
        }
    }
}
