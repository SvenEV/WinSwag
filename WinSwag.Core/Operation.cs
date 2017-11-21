using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinSwag.Core
{
    public class Operation
    {
        public SwaggerOperationDescription Specification { get; }

        public string OperationId => $"{Specification.Method.ToString().ToUpper()} {Specification.Path}";

        public IReadOnlyList<IParameter> Parameters { get; }

        public string RequestContentType { get; } // TODO

        public IEnumerable<string> AcceptedContentTypes => Specification.Operation.ActualConsumes;

        public Operation(SwaggerOperationDescription operation, DocumentCreationContext context)
        {
            Specification = operation ?? throw new ArgumentNullException(nameof(operation));

            var defaultContentType = Specification.Operation.ActualConsumes?.FirstOrDefault() ?? "application/json";

            Parameters = operation.Operation.Parameters
                .Select(p => Parameter.FromSpec(p, this, defaultContentType, context))
                .ToList();
        }
    }
}
