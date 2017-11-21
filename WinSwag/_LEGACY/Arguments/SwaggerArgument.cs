//using GalaSoft.MvvmLight;
//using Newtonsoft.Json.Linq;
//using NJsonSchema;
//using NSwag;
//using System;
//using System.Diagnostics;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;

//namespace WinSwag.Models.Arguments
//{
//    [DebuggerDisplay("{ParameterId}")]
//    public abstract class SwaggerArgument : ObservableObject
//    {
//        public SwaggerParameter Parameter { get; }

//        public string ParameterId => $"{Parameter.Name}:{Parameter.Kind}";

//        public string DisplayName => Parameter.Name + (Parameter.IsRequired ? "*" : "");

//        public bool HasDescription => !string.IsNullOrWhiteSpace(Parameter.Description);

//        public bool IsBodyParameter => Parameter.Kind == SwaggerParameterKind.Body;

//        public abstract bool HasValue { get; }

//        public SwaggerArgument(SwaggerParameter parameter)
//        {
//            Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
//        }

//        /// <param name="request">The request message to populate with arguments</param>
//        /// <param name="requestUri">The current request URI, ending with either '?' or '&' so query parameters can just be appended</param>
//        /// <param name="mediaType">The selected media type to be used in the request</param>
//        public abstract Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri, string contentType);

//        public static ISwaggerArgument FromParameter(SwaggerParameter param)
//        {
//            if (param == null)
//                throw new ArgumentNullException(nameof(param));

//            if (param.Type == JsonObjectType.Boolean)
//                return new BoolArgument();

//            if (param.Format == "date-time")
//                return new DateTimeArgument();

//            if (param.ActualSchema.IsEnumeration)
//                return new EnumArgument(param);

//            if (param.Type == JsonObjectType.File)
//                return new FileArgument();

//            return new StringArgument();
//        }

//        public abstract JToken GetSerializedValue();

//        public abstract Task SetSerializedValueAsync(JToken o);
//    }
//}
