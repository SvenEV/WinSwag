using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public interface IArgument
    {
        IParameter Parameter { get; }
        object ObjectValue { get; set; }
        bool HasValue { get; }
        string ContentType { get; set; } // only applicable to body-parameters

        Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri);
        JToken GetSerializedValue();
        Task SetSerializedValueAsync(JToken o);
    }

    //public interface IArgument<T> : IArgument
    //{
    //    new IParameter<T> Parameter { get; }
    //    new T Value { get; set; }
    //}
}
