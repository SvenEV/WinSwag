using Newtonsoft.Json.Linq;
using NJsonSchema;
using System;
using System.Linq;

namespace WinSwag.Core
{
    /// <summary>
    /// Generates sample JSON data from a JSON schema.
    /// </summary>
    public class JsonExampleGenerator
    {
        public static JToken CreateSample(JsonSchema4 schema)
        {
            schema = schema.ActualSchema;

            if (schema.IsAnyType)
                return null;

            switch (schema.Type)
            {
                case JsonObjectType.Boolean: return true;
                case JsonObjectType.File: return null;
                case JsonObjectType.Integer: return 0;
                case JsonObjectType.Null: return null;
                case JsonObjectType.Number: return 0.0;

                case JsonObjectType.String:
                    if (schema.ExtensionData != null && schema.ExtensionData.TryGetValue("example", out var example))
                        return example?.ToString() ?? "string";

                    if (schema.Enumeration?.Any() ?? false)
                        return schema.Enumeration.First()?.ToString() ?? "string";

                    switch (schema.Format)
                    {
                        case "date": return DateTimeOffset.Now.ToString("yyyy-MM-dd");
                        case "date-time": return DateTimeOffset.Now.ToString("s");
                        default: return "string";
                    }

                case JsonObjectType.Array:
                    return new JArray(CreateSample(schema.Item));

                case JsonObjectType.Object:
                    var obj = new JObject();
                    
                    foreach (var prop in schema.ActualProperties)
                    {
                        obj[prop.Key] = CreateSample(prop.Value);
                    }

                    return obj;

                case JsonObjectType.None: throw new ArgumentException("Invalid type", nameof(schema));
                default: throw new NotImplementedException();
            }
        }
    }
}
