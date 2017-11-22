using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class Session
    {
        public SessionInfo Info { get; set; }

        public Dictionary<string, StoredOperation> Operations { get; set; } = new Dictionary<string, StoredOperation>();

        public Dictionary<string, StoredArgument> GlobalArguments { get; set; } = new Dictionary<string, StoredArgument>();

        public bool ShouldSerializeOperations() => Operations.Count > 0;

        public bool ShouldSerializeGlobalArguments() => GlobalArguments.Count > 0;

        public static Session FromDocument(OpenApiDocument doc)
        {
            return new Session
            {
                Info = new SessionInfo(doc.DisplayName, doc.SourceUrl),

                GlobalArguments = doc.GlobalArguments
                    .Select(arg => new { Key = arg.Parameter.ParameterId, Value = new StoredArgument(arg) })
                    .Where(o => o.Value.ShouldSerialize())
                    .ToDictionary(o => o.Key, o => o.Value),

                Operations = doc.OperationGroups
                    .SelectMany(g => g)
                    .Select(op => new
                    {
                        Key = op.OperationId,
                        Value = new StoredOperation
                        {
                            Arguments = op.Parameters
                                .Select(p => new { Key = p.ParameterId, Value = new StoredArgument(p.LocalArgument) })
                                .Where(o => o.Value.ShouldSerialize())
                                .ToDictionary(o => o.Key, o => o.Value)
                        }
                    })
                    .Where(o => o.Value.ShouldSerialize())
                    .ToDictionary(o => o.Key, o => o.Value)
            };
        }

        public static async Task<OpenApiDocument> ToDocumentAsync(Session session, OpenApiSettings settings = null)
        {
            settings = settings ?? OpenApiSettings.Default;

            var doc = await OpenApiDocument.LoadFromUrlAsync(session.Info.Url, settings);
            doc.DisplayName = session.Info.DisplayName ?? doc.DisplayName;

            // Restore global arguments
            foreach (var storedArg in session.GlobalArguments)
            {
                var argument = doc.GlobalArguments
                    .FirstOrDefault(arg => arg.Parameter.ParameterId == storedArg.Key);

                if (argument != null)
                {
                    argument.IsActive = storedArg.Value.IsActive;
                    await argument.SetSerializedValueAsync(storedArg.Value.Value);
                }
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
                        parameter.LocalArgument.IsActive = storedArg.Value.IsActive;
                        await parameter.LocalArgument.SetSerializedValueAsync(storedArg.Value.Value);
                    }
                }
            }

            return doc;
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

        public static Session FromJson(string json) => JsonConvert.DeserializeObject<Session>(json);


        public class StoredOperation
        {
            public Dictionary<string, StoredArgument> Arguments { get; set; } = new Dictionary<string, StoredArgument>();

            public bool ShouldSerialize() => Arguments?.Count > 0;
        }

        public class StoredArgument
        {
            private readonly bool _hasInitialValue;

            [DefaultValue(true)]
            [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
            public bool IsActive { get; set; }

            public JToken Value { get; set; }

            public bool ShouldSerializeValue() => !_hasInitialValue;

            public bool ShouldSerialize() => !IsActive || !_hasInitialValue;

            public StoredArgument()
            {
            }

            public StoredArgument(IArgument argument)
            {
                IsActive = argument.IsActive;
                Value = argument.GetSerializedValue();
                _hasInitialValue = Equals(argument.ObjectValue, argument.InitialValue);
            }
        }
    }
}
