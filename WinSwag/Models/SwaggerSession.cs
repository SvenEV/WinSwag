using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSwag;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WinSwag.ViewModels;

namespace WinSwag.Models
{
    public class SwaggerSession
    {
        public string Url { get; set; }

        public string DisplayName { get; set; }

        public Dictionary<string, StoredOperation> Operations { get; set; }

        public static SwaggerSession FromViewModel(SwaggerDocumentViewModel vm, string displayName)
        {
            return new SwaggerSession
            {
                Url = vm.Url,
                DisplayName = displayName,
                Operations = vm.OperationGroups
                    .SelectMany(g => g)
                    .ToDictionary(
                        op => op.OperationId,
                        op => new StoredOperation
                        {
                            ContentType = op.SelectedContentType,
                            Arguments = op.Arguments
                                .Where(p => p.HasValue)
                                .ToDictionary(p => p.ParameterId, p => p.GetSerializedValue())
                        })
            };
        }

        public static async Task<SwaggerDocumentViewModel> ToViewModelAsync(SwaggerSession session)
        {
            var doc = await SwaggerDocumentLoader.LoadFromUrlAsync(session.Url);
            var vm = new SwaggerDocumentViewModel(doc, session.Url, session.DisplayName);

            foreach (var storedOp in session.Operations)
            {
                var operation = vm.OperationGroups
                    .SelectMany(g => g)
                    .FirstOrDefault(op => op.OperationId == storedOp.Key);

                if (operation != null)
                {
                    operation.SelectedContentType = storedOp.Value.ContentType;

                    foreach (var storedArg in storedOp.Value.Arguments)
                    {
                        var parameter = operation.Arguments.FirstOrDefault(p => p.ParameterId == storedArg.Key);
                        await parameter.SetSerializedValueAsync(storedArg.Value);
                    }
                }
            }

            return vm;
        }

        public string ToJson() => JsonConvert.SerializeObject(this);

        public static SwaggerSession FromJson(string json) => JsonConvert.DeserializeObject<SwaggerSession>(json);

        public class StoredOperation
        {
            public string ContentType { get; set; }
            public Dictionary<string, JToken> Arguments { get; set; }
        }
    }
}
