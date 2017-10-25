using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace WinSwag.Models
{
    public class Parameter
    {
        public string Name { get; }

        public string Description { get; }

        [JsonProperty("in")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ParameterLocation Location { get; }

        [JsonProperty("required")]
        public bool IsRequired { get; }

        public string Type { get; }

        public JObject Schema { get; }

        public string Format { get; }

        public Parameter(string name, string description, ParameterLocation location, bool isRequired, string type, JObject schema, string format)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Location = location;
            IsRequired = isRequired;
            Type = type;
            Schema = schema;
            Format = format;
        }
    }

    public enum ParameterLocation
    {
        /// <summary>
        /// URL parameter, e.g. in "/api/User/{id}"
        /// </summary>
        Path,

        /// <summary>
        /// Query parameter, e.g. in "/api/Thumbnail?size=medium".
        /// </summary>
        Query,

        /// <summary>
        /// Form data parameter.
        /// </summary>
        FormData,

        /// <summary>
        /// Header parameter, e.g. authorization header.
        /// </summary>
        Header,

        /// <summary>
        /// Body parameter.
        /// </summary>
        Body
    }
}
