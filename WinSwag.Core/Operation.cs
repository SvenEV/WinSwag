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

        public IEnumerable<string> AcceptedContentTypes => Specification.Operation.ActualConsumes;

        public Operation(SwaggerOperationDescription operation, DocumentCreationContext context)
        {
            Specification = operation ?? throw new ArgumentNullException(nameof(operation));
            Document = context.Document;

            context.CurrentOperation = this;

            Parameters = operation.Operation.Parameters
                .Select(p => new Parameter(p, context))
                .ToList();

            context.CurrentOperation = null;
        }
    }
}
