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

        public BoolArgument(SwaggerParameter parameter) : base(parameter)
        {
        }

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri) =>
            StringArgument.ApplyAsync(Parameter, _value.ToString(), request, requestUri);
    }
}
