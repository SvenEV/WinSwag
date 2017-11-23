using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public interface IArgument : INotifyPropertyChangedEx
    {
        Parameter Parameter { get; }

        IReadOnlyList<Parameter> Parameters { get; }

        /// <summary>
        /// Indicates whether this argument should be included in the request.
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// The current value as object.
        /// </summary>
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

        /// <summary>
        /// Applies the argument to the HTTP request body or request URI
        /// (depending on the kind of parameter) and sets the necessary headers.
        /// </summary>
        Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri);

        /// <summary>
        /// Converts the current argument value to a serializable JSON-value that
        /// can be stored as part of a session.
        /// </summary>
        JToken GetSerializedValue();

        /// <summary>
        /// Converts the specified serializable JSON-value to the argument's internal representation
        /// and makes it the current value of the argument.
        /// </summary>
        Task SetSerializedValueAsync(JToken o);
    }
}
