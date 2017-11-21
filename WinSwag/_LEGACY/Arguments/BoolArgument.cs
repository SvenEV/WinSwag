using GalaSoft.MvvmLight;
using Newtonsoft.Json.Linq;
using NSwag;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core.Arguments
{
    public class BoolArgument : ObservableObject, ISwaggerArgument<bool>
    {
        private bool _value;

        public bool Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public bool HasValue => true; // TODO: Make value of type bool? and serialize only if not null

        public Task ApplyAsync(SwaggerParameter parameter, HttpRequestMessage request, StringBuilder requestUri, string contentType) =>
            StringArgument.ApplyAsync(parameter, _value.ToString(), request, requestUri, contentType);

        public JToken GetSerializedValue() => JToken.FromObject(Value);

        public Task SetSerializedValueAsync(JToken o) => Task.FromResult(Value = o.ToObject<bool>());
    }
}
