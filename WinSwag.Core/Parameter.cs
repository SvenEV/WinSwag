using NSwag;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class Parameter
    {
        public SwaggerParameter Specification { get; }

        public Operation Operation { get; }

        public string ParameterId => Id(Specification);

        public IArgument LocalArgument { get; }

        public IArgument GlobalArgument { get; internal set; }

        public Parameter(SwaggerParameter parameter, Operation operation, ArgumentBase localArgument, IArgument globalArgument)
        {
            Specification = parameter ?? throw new ArgumentNullException(nameof(parameter));
            Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            LocalArgument = localArgument ?? throw new ArgumentNullException(nameof(localArgument));
            GlobalArgument = globalArgument ?? throw new ArgumentNullException(nameof(globalArgument));
            localArgument.Init(new[] { this });
        }

        public async Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri)
        {
            if (LocalArgument.IsActive)
                await LocalArgument.ApplyAsync(request, requestUri);
            else if (GlobalArgument.IsActive)
                await GlobalArgument.ApplyAsync(request, requestUri);
        }

        public static string Id(SwaggerParameter spec) => $"{spec.Name}:{spec.Kind}";

        public static Parameter FromSpec(SwaggerParameter spec, string defaultContentType, DocumentCreationContext context)
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));

            var localArgument = (ArgumentBase)Argument.FromSpec(spec, defaultContentType, context.Settings);
            var globalArgument = context.GlobalArguments.GetOrAdd(Id(spec), _ => Argument.FromSpec(spec, defaultContentType, context.Settings));
            return new Parameter(spec, context.CurrentOperation, localArgument, globalArgument);
        }
    }
}
