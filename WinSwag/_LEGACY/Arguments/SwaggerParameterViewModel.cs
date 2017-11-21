using Newtonsoft.Json.Linq;
using NJsonSchema;
using NSwag;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core.Arguments
{
    [DebuggerDisplay("{ParameterId}")]
    public sealed class SwaggerParameterViewModel
    {
        public SwaggerParameter Parameter { get; }

        public ISwaggerArgument<object> Argument { get; }
        
        // Next, higher-level parameter
        public SwaggerParameterViewModel Parent { get; }

        public object DefaultValue => Parent == null
            ? (Argument.HasValue ? Argument.Value : Parameter.Default)
            : Parent.DefaultValue;

        public string ParameterId => $"{Parameter.Name}:{Parameter.Kind}";

        public string DisplayName => Parameter.Name + (Parameter.IsRequired ? "*" : "");

        public bool HasDescription => !string.IsNullOrWhiteSpace(Parameter.Description);

        public bool IsBodyParameter => Parameter.Kind == SwaggerParameterKind.Body;

        public SwaggerParameterViewModel(SwaggerParameter parameter, SwaggerParameterViewModel parent)
        {
            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Parent = parent;
            Argument = SwaggerArgument.FromParameter(parameter);
        }

        /// <param name="request">The request message to populate with arguments</param>
        /// <param name="requestUri">The current request URI, ending with either '?' or '&' so query parameters can just be appended</param>
        /// <param name="mediaType">The selected media type to be used in the request</param>
        public async Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri, string contentType)
        {
            await (Argument.HasValue ? Argument : Parent?.Argument ?? Argument)
                .ApplyAsync(Parameter, request, requestUri, contentType);
        }

    }

    public interface ISwaggerArgument<out T>
    {
        T Value { get; }
        bool HasValue { get; }
        Task ApplyAsync(SwaggerParameter parameter, HttpRequestMessage request, StringBuilder requestUri, string contentType);
        JToken GetSerializedValue();
        Task SetSerializedValueAsync(JToken o);
    }

    public static class SwaggerArgument
    {
        public static ISwaggerArgument<object> FromParameter(SwaggerParameter param)
        {
            if (param == null)
                throw new ArgumentNullException(nameof(param));

            if (param.Type == JsonObjectType.Boolean)
                return (ISwaggerArgument<object>)new BoolArgument();

            if (param.Format == "date-time")
                return (ISwaggerArgument<object>)new DateTimeArgument();

            if (param.ActualSchema.IsEnumeration)
                return new EnumArgument(param);

            if (param.Type == JsonObjectType.File)
                return new FileArgument();

            return new StringArgument();
        }
    }
}
