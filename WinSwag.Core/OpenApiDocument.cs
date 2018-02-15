using Newtonsoft.Json;
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
        private SwaggerDocument Specification { get; }

        public IReadOnlyList<IArgument> GlobalArguments { get; }

        public IReadOnlyList<OperationGroup> OperationGroups { get; }

        public IReadOnlyList<ISecurityScheme> SecuritySchemes { get; }

        public string Description => Specification.Info?.Description?.Trim();

        public string BaseUrl => Specification.BaseUrl;

        /// <summary>
        /// The URL to the JSON- or YAML-file this document was loaded from.
        /// </summary>
        public string SourceUrl { get; }

        public string DisplayName { get; set; }

        public string Version => Specification.Info.Version;

        public OpenApiDocument(SwaggerDocument specification, string sourceUrl, OpenApiSettings settings = null)
        {
            Specification = specification ?? throw new ArgumentNullException(nameof(specification));
            SourceUrl = sourceUrl ?? throw new ArgumentNullException(nameof(sourceUrl));
            DisplayName = specification.Info.Title;
            settings = settings ?? OpenApiSettings.Default;

            var context = new DocumentCreationContext(this, settings);

            var operations = specification.Operations
                .ToDictionary(op => op.Operation, op => new Operation(op, context));

            OperationGroups = specification.Operations
                .OrderBy(op => op.Path)
                .Select(op => operations[op.Operation])
                .GroupBy(op => op.GroupName)
                .Select(group => new OperationGroup(group.Key, group))
                .OrderBy(group => group.Name)
                .ToList();

            // set up relationship "global argument -> parameters"
            foreach (var arg in context.GlobalArguments)
            {
                var parameters = operations.Values
                    .SelectMany(op => op.Parameters)
                    .Where(p => p.ParameterId == arg.Key);

                ((dynamic)arg.Value).Init(parameters); // TODO: de-uglify!
            }

            GlobalArguments = operations.Values
                .SelectMany(op => op.Parameters)
                .Select(param => param.GlobalArgument)
                .Distinct()
                .Where(arg => arg.Parameters.Count > 1)
                .OrderBy(arg => arg.Parameter.Name)
                .ToList(); // TODO: Select only those that appear frequently

            SecuritySchemes = specification.SecurityDefinitions
                .Select(kvp => SecurityScheme.FromSpec(kvp.Key, kvp.Value))
                .ToList();
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
                var doc = await SwaggerDocument.FromJsonAsync(data);
                return new OpenApiDocument(doc, sourceUrl, settings);
            }
            catch (JsonException)
            {
                try
                {
                    var doc = await SwaggerYamlDocument.FromYamlAsync(data);
                    return new OpenApiDocument(doc, sourceUrl, settings);
                }
                catch (JsonException)
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
