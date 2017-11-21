using NSwag;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;

[assembly: DebuggerDisplay("{Method} {Path,nq}", Target = typeof(SwaggerOperationDescription))]
[assembly: DebuggerDisplay("{Name,nq}{IsRequired ? \"*\" : \"\",nq} in {Kind}", Target = typeof(SwaggerParameter))]

namespace WinSwag.Core
{
    public static class NSwagExtensions
    {
        private static readonly Dictionary<SwaggerOperationMethod, HttpMethod> _methodMappings = new Dictionary<SwaggerOperationMethod, HttpMethod>
        {
            { SwaggerOperationMethod.Undefined, null },
            { SwaggerOperationMethod.Get, HttpMethod.Get },
            { SwaggerOperationMethod.Post, HttpMethod.Post },
            { SwaggerOperationMethod.Put, HttpMethod.Put },
            { SwaggerOperationMethod.Delete, HttpMethod.Delete },
            { SwaggerOperationMethod.Options, HttpMethod.Options },
            { SwaggerOperationMethod.Head, HttpMethod.Head },
            { SwaggerOperationMethod.Patch, new HttpMethod("PATCH") }
        };

        public static HttpMethod ToHttpMethod(this SwaggerOperationMethod method) => _methodMappings[method];
    }
}
