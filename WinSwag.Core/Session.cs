using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class Session
    {
        public string Url { get; set; }

        public string DisplayName { get; set; }

        public Dictionary<string, StoredOperation> Operations { get; set; } = new Dictionary<string, StoredOperation>();

        public Dictionary<string, JToken> GlobalArguments { get; set; } = new Dictionary<string, JToken>();

        public static Session FromDocument(OpenApiDocument doc, string displayName)
        {
            return new Session
            {
                Url = doc.SourceUrl,
                DisplayName = displayName,

                GlobalArguments = doc.GlobalArguments
                    .ToDictionary(arg => arg.Parameter.ParameterId, arg => arg.GetSerializedValue()),

                Operations = doc.OperationGroups
                    .SelectMany(g => g)
                    .ToDictionary(
                        op => op.OperationId,
                        op => new StoredOperation
                        {
                            Arguments = op.Parameters
                                .Where(p => p.LocalArgument.HasValue)
                                .ToDictionary(p => p.ParameterId, p => p.LocalArgument.GetSerializedValue())
                        })
            };
        }

        public static async Task<OpenApiDocument> ToDocumentAsync(Session session, OpenApiSettings settings = null)
        {
            settings = settings ?? OpenApiSettings.Default;

            var doc = await OpenApiDocument.LoadFromUrlAsync(session.Url, settings);

            // Restore global arguments
            foreach (var storedArg in session.GlobalArguments)
            {
                var argument = doc.GlobalArguments
                    .FirstOrDefault(arg => arg.Parameter.ParameterId == storedArg.Key);

                if (argument != null)
                    await argument.SetSerializedValueAsync(storedArg.Value);
            }

            // Restore local arguments
            foreach (var storedOp in session.Operations)
            {
                var operation = doc.OperationGroups
                    .SelectMany(g => g)
                    .FirstOrDefault(op => op.OperationId == storedOp.Key);

                if (operation != null)
                {
                    foreach (var storedArg in storedOp.Value.Arguments)
                    {
                        var parameter = operation.Parameters.FirstOrDefault(p => p.ParameterId == storedArg.Key);
                        await parameter.LocalArgument.SetSerializedValueAsync(storedArg.Value);
                    }
                }
            }

            return doc;
        }

        public string ToJson() => JsonConvert.SerializeObject(this);

        public static Session FromJson(string json) => JsonConvert.DeserializeObject<Session>(json);

        public class StoredOperation
        {
            public Dictionary<string, JToken> Arguments { get; set; }
        }
    }
}
