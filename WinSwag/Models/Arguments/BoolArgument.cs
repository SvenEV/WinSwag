using Newtonsoft.Json.Linq;
using NSwag;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Models.Arguments
{
    public class BoolArgument : SwaggerArgument
    {
        private bool _value;

        public bool Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public override bool HasValue => true; // TODO: Make value of type bool? and serialize only if not null

        public BoolArgument(SwaggerParameter parameter) : base(parameter)
        {
        }

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri) =>
            StringArgument.ApplyAsync(Parameter, _value.ToString(), request, requestUri);

        public override JToken GetSerializedValue() => JToken.FromObject(Value);

        public override Task SetSerializedValueAsync(JToken o) => Task.FromResult(Value = o.ToObject<bool>());
    }
}
