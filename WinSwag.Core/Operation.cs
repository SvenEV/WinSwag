using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinSwag.Core
{
    public class Operation
    {
        public SwaggerOperationDescription Specification { get; }

        public OpenApiDocument Document { get; }

        public string OperationId => $"{Specification.Method.ToString().ToUpper()} {Specification.Path}";

        public IReadOnlyList<Parameter> Parameters { get; }

        public string RequestContentType { get; } // TODO

        public IEnumerable<string> AcceptedContentTypes => Specification.Operation.ActualConsumes;

        public Operation(SwaggerOperationDescription operation, DocumentCreationContext context)
        {
            Specification = operation ?? throw new ArgumentNullException(nameof(operation));
            Document = context.Document;

            context.CurrentOperation = this;
            var defaultContentType = Specification.Operation.ActualConsumes?.FirstOrDefault() ?? "application/json";

            Parameters = operation.Operation.Parameters
                .Select(p => Parameter.FromSpec(p, defaultContentType, context))
                .ToList();

            context.CurrentOperation = null;
        }
    }
}
