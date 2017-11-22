using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public interface IArgument : INotifyPropertyChangedEx
    {
        Parameter Parameter { get; }

        /// <summary>
        /// Indicates whether this argument should be included in the request.
        /// </summary>
        bool IsActive { get; set; }

        object ObjectValue { get; set; }

        /// <summary>
        /// Indicates whether a value is assigned that is not the initial value that this argument has
        /// when loading an OpenAPI document for the first time.
        /// </summary>
        object InitialValue { get; }

        /// <summary>
        /// The media type of the value. Only applicable to body-parameters.
        /// </summary>
        string ContentType { get; set; }

        Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri);
        JToken GetSerializedValue();
        Task SetSerializedValueAsync(JToken o);
    }
}
