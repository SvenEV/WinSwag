using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WinSwag.Models
{
    public class Operation
    {
        [JsonProperty("operationId")]
        public string Id { get; }

        public string Description { get; }

        [JsonIgnore]
        public string Path { get; internal set; }


        [JsonIgnore]
        public HttpVerb Method { get; internal set; }

        public IReadOnlyList<Parameter> Parameters { get; }

        public IReadOnlyDictionary<string, Response> Responses { get; }

        public IReadOnlyList<string> Tags { get; }

        public Operation(string id, string description, IReadOnlyList<Parameter> parameters, IReadOnlyDictionary<string, Response> responses, IReadOnlyList<string> tags)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Description = description;
            Parameters = parameters ?? Array.Empty<Parameter>();
            Responses = responses ?? throw new ArgumentNullException(nameof(responses));
            Tags = tags ?? throw new ArgumentNullException(nameof(tags));

            foreach (var response in responses)
                response.Value.StatusCode = response.Key;
        }

        public override string ToString() => $"{Method.ToString().ToUpper()} {Path}";
    }
}
