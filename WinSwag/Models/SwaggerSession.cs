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

        public Dictionary<string, Dictionary<string, JToken>> Arguments { get; set; }

        public static SwaggerSession FromViewModel(SwaggerSpecificationViewModel vm)
        {
            return new SwaggerSession
            {
                Url = vm.Url,
                Arguments = vm.OperationGroups
                    .SelectMany(g => g)
                    .ToDictionary(
                        op => op.Model.Operation.OperationId,
                        op => op.Arguments.ToDictionary(p => p.Parameter.Name, p => p.GetSerializedValue()))
            };
        }

        public static async Task<SwaggerSpecificationViewModel> ToViewModelAsync(SwaggerSession session)
        {
            var doc = await SwaggerDocument.FromUrlAsync(session.Url);
            var vm = new SwaggerSpecificationViewModel(doc, session.Url);

            foreach (var storedOp in session.Arguments)
            {
                var operation = vm.OperationGroups
                    .SelectMany(g => g)
                    .FirstOrDefault(op => op.Model.Operation.OperationId == storedOp.Key);

                if (operation != null)
                {
                    foreach (var storedArg in storedOp.Value)
                    {
                        var parameter = operation.Arguments.FirstOrDefault(p => p.Parameter.Name == storedArg.Key);
                        await parameter.SetSerializedValueAsync(storedArg.Value);
                    }
                }
            }

            return vm;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static SwaggerSession FromJson(string json)
        {
            return JsonConvert.DeserializeObject<SwaggerSession>(json);
        }
    }
}
