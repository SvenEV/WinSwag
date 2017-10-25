using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WinSwag.Models
{
    public class SwaggerSpecification
    {
        public string Host { get; }

        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, Operation>> Paths { get; }

        public IReadOnlyDictionary<string, Definition> Definitions { get; }

        public IReadOnlyDictionary<string, OperationGroup> OperationGroups { get; }

        public SwaggerSpecification(string host, IReadOnlyDictionary<string, IReadOnlyDictionary<string, Operation>> paths, IReadOnlyDictionary<string, Definition> definitions)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host));
            Paths = paths ?? throw new ArgumentNullException(nameof(paths));
            Definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));

            OperationGroups = new[]
            {
                new OperationGroup("Default Group", Paths.Values.SelectMany(p => p.Values).ToList())
            }.ToDictionary(group => group.Name);

            foreach (var path in Paths)
            {
                foreach (var op in path.Value)
                {
                    op.Value.Method = Enum.Parse<HttpVerb>(op.Key, ignoreCase: true);
                    op.Value.Path = path.Key;
                }
            }
        }

        public static async Task<SwaggerSpecification> LoadAsync(string url)
        {
            using (var http = new HttpClient())
            {
                var json = await http.GetStringAsync(url);
                return await Task.Run(() => JsonConvert.DeserializeObject<SwaggerSpecification>(json));
            }
        }
    }

    public class Response
    {
        [JsonIgnore]
        public string StatusCode { get; internal set; }

        public string Description { get; }
    }

    public enum HttpVerb
    {
        Get,
        Post,
        Put,
        Delete,
        Head,
        Options,
        Trace
    }

    public static class HttpVerbExtensions
    {
        public static HttpMethod ToHttpMethod(this HttpVerb verb)
        {
            switch (verb)
            {
                case HttpVerb.Get: return HttpMethod.Get;
                case HttpVerb.Post: return HttpMethod.Post;
                case HttpVerb.Put: return HttpMethod.Put;
                case HttpVerb.Delete: return HttpMethod.Delete;
                case HttpVerb.Head: return HttpMethod.Head;
                case HttpVerb.Options: return HttpMethod.Options;
                case HttpVerb.Trace: return HttpMethod.Trace;
                default: throw new NotImplementedException();
            }
        }
    }

    public class Definition
    {

    }
}
