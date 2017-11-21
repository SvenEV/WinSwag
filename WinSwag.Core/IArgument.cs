using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public interface IArgument : INotifyPropertyChanged
    {
        Parameter Parameter { get; }

        /// <summary>
        /// Indicates whether this argument should be included in the request.
        /// </summary>
        bool IsActive { get; set; }

        object ObjectValue { get; set; }

        /// <summary>
        /// Indicates whether a value is assigned that is not the default value for the argument type.
        /// TODO: This should really only check for type's default, e.g. false, null, 0.
        ///       It should have nothing to do with swagger-spec's default value.
        /// </summary>
        bool HasNonDefaultValue { get; }

        /// <summary>
        /// The media type of the value. Only applicable to body-parameters.
        /// </summary>
        string ContentType { get; set; }

        Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri);
        JToken GetSerializedValue();
        Task SetSerializedValueAsync(JToken o);
    }
}
