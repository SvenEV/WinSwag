using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class OpenApiDocument
    {
        public SwaggerDocument Specification { get; }

        public IReadOnlyList<IArgument> GlobalArguments { get; }

        public IReadOnlyList<OperationGroup> OperationGroups { get; }

        public string Description => Specification.Info?.Description?.Trim();

        public string SourceUrl { get; }

        public OpenApiDocument(SwaggerDocument specification, string sourceUrl, OpenApiSettings settings = null)
        {
            Specification = specification ?? throw new ArgumentNullException(nameof(specification));
            SourceUrl = sourceUrl ?? throw new ArgumentNullException(nameof(sourceUrl));

            settings = settings ?? OpenApiSettings.Default;

            var context = new DocumentCreationContext(this, settings);

            var operations = specification.Operations
                .ToDictionary(op => op.Operation, op => new Operation(op, context));

            OperationGroups = specification.Operations
                .OrderBy(op => op.Path)
                .Select(op => operations[op.Operation])
                .GroupBy(op => op.Specification.Operation.Tags.FirstOrDefault() ?? "(Default)")
                .Select(group => new OperationGroup(group.Key, group))
                .OrderBy(group => group.Name)
                .ToList();

            foreach (var arg in context.GlobalArguments)
            {
                var parameters = operations.Values
                    .SelectMany(op => op.Parameters)
                    .Where(p => p.ParameterId == arg.Key);

                ((dynamic)arg.Value).Init(parameters);
            }

            GlobalArguments = operations.Values
                .SelectMany(op => op.Parameters)
                .GroupBy(param => (param.Specification.Name, param.Specification.Kind))
                .Select(group => group.First().GlobalArgument)
                .ToList(); // TODO: Select only those that appear frequently
        }

        public static async Task<OpenApiDocument> LoadFromUrlAsync(string url, OpenApiSettings settings = null)
        {
            settings = settings ?? OpenApiSettings.Default;
            var http = new HttpClient();
            var response = await http.GetAsync(url);

            if (response.Content == null)
                throw new InvalidOperationException("Request did not return any content");

            var data = await response.Content.ReadAsStringAsync();

            switch (response.Content.Headers.ContentType?.MediaType)
            {
                case "text/json": return new OpenApiDocument(await SwaggerDocument.FromJsonAsync(data), url, settings);
                case "text/yaml": return new OpenApiDocument(await SwaggerYamlDocument.FromYamlAsync(data), url, settings);

                // If no proper content type is provided, try both: JSON, then YAML
                default: return await LoadFromStringAsync(data, url, settings);
            }
        }

        public static async Task<OpenApiDocument> LoadFromStringAsync(string data, string sourceUrl, OpenApiSettings settings = null)
        {
            settings = settings ?? OpenApiSettings.Default;
            try
            {
                return new OpenApiDocument(await SwaggerDocument.FromJsonAsync(data), sourceUrl, settings);
            }
            catch
            {
                try
                {
                    return new OpenApiDocument(await SwaggerYamlDocument.FromYamlAsync(data), sourceUrl, settings);
                }
                catch
                {
                    throw new InvalidOperationException("Could not determine content type");
                }
            }
        }
    }

    public class DocumentCreationContext
    {
        public OpenApiSettings Settings { get; }

        public OpenApiDocument Document { get; }

        public Operation CurrentOperation { get; set; }

        public IDictionary<string, IArgument> GlobalArguments { get; } = new Dictionary<string, IArgument>();

        public DocumentCreationContext(OpenApiDocument document, OpenApiSettings settings = null)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            Settings = settings ?? OpenApiSettings.Default;
        }
    }
}
