using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinSwag.Core
{
    public class Operation
    {
        private SwaggerOperationDescription Specification { get; }

        public OpenApiDocument Document { get; }

        /// <summary>
        /// The ID of the operation. Consists of <see cref="Method"/> and <see cref="Path"/>.
        /// </summary>
        public string OperationId => $"{Method.ToString().ToUpper()} {Path}";

        public string Description => string.IsNullOrWhiteSpace(Specification.Operation.Summary)
            ? Specification.Operation.Description
            : Specification.Operation.Summary;

        public string Path => Specification.Path;

        public SwaggerOperationMethod Method => Specification.Method;

        public string GroupName => Specification.Operation.Tags.FirstOrDefault() ?? "(Default)";

        public IReadOnlyList<Parameter> Parameters { get; }

        public IEnumerable<string> AcceptedContentTypes =>
            Specification.Operation.ActualConsumes ?? Enumerable.Empty<string>();

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
