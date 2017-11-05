using GalaSoft.MvvmLight;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using NSwag;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Models.Arguments
{
    [DebuggerDisplay("{Parameter}")]
    public abstract class SwaggerArgument : ObservableObject
    {
        public SwaggerParameter Parameter { get; }

        public string DisplayName => Parameter.Name + (Parameter.IsRequired ? "*" : "");

        public bool IsBodyParameter => Parameter.Kind == SwaggerParameterKind.Body;

        public SwaggerArgument(SwaggerParameter parameter)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        /// <param name="request">The request message to populate with arguments</param>
        /// <param name="requestUri">The current request URI, ending with either '?' or '&' so query parameters can just be appended</param>
        public abstract Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri);

        public static SwaggerArgument FromParameter(SwaggerParameter param)
        {
            if (param == null)
                throw new ArgumentNullException(nameof(param));

            if (param.Type == JsonObjectType.Boolean)
                return new BoolArgument(param);

            if (param.Format == "date-time")
                return new DateTimeArgument(param);

            if (param.ActualSchema.IsEnumeration)
                return new EnumArgument(param);

            if (param.Type == JsonObjectType.File)
                return new FileArgument(param);

            return new StringArgument(param);
        }

        public abstract JToken GetSerializedValue();

        public abstract Task SetSerializedValueAsync(JToken o);
    }
}
