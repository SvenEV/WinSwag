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

        public Dictionary<string, StoredOperation> Operations { get; set; }

        public static SwaggerSession FromViewModel(SwaggerDocumentViewModel vm)
        {
            return new SwaggerSession
            {
                Url = vm.Url,
                Operations = vm.OperationGroups
                    .SelectMany(g => g)
                    .ToDictionary(
                        op => op.Model.Operation.OperationId,
                        op => new StoredOperation
                        {
                            ContentType = op.SelectedContentType,
                            Arguments = op.Arguments.ToDictionary(p => p.Parameter.Name, p => p.GetSerializedValue())
                        })
            };
        }

        public static async Task<SwaggerDocumentViewModel> ToViewModelAsync(SwaggerSession session)
        {
            var doc = await SwaggerDocument.FromUrlAsync(session.Url);
            var vm = new SwaggerDocumentViewModel(doc, session.Url);

            foreach (var storedOp in session.Operations)
            {
                var operation = vm.OperationGroups
                    .SelectMany(g => g)
                    .FirstOrDefault(op => op.Model.Operation.OperationId == storedOp.Key);

                if (operation != null)
                {
                    operation.SelectedContentType = storedOp.Value.ContentType;

                    foreach (var storedArg in storedOp.Value.Arguments)
                    {
                        var parameter = operation.Arguments.FirstOrDefault(p => p.Parameter.Name == storedArg.Key);
                        await parameter.SetSerializedValueAsync(storedArg.Value);
                    }
                }
            }

            return vm;
        }

        public class StoredOperation
        {
            public string ContentType { get; set; }
            public Dictionary<string, JToken> Arguments { get; set; }
        }
    }
}
