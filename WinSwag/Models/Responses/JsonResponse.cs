using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WinSwag.Models.Responses
{
    public class JsonResponse : ResponseViewModel
    {
        public string Json { get; private set; }

        public JsonResponse(HttpResponseMessage response, string requestUri) : base(response, requestUri) { }

        public static new async Task<JsonResponse> FromResponseAsync(HttpResponseMessage response, string requestUri)
        {
            var json = await response.Content.ReadAsStringAsync();

            // Try formatting as indented JSON (in background thread)
            await Task.Run(() =>
            {
                try
                {
                    var o = JToken.Parse(json);
                    json = o.ToString(Formatting.Indented);
                }
                catch (JsonReaderException)
                {
                }
            });

            return new JsonResponse(response, requestUri) { Json = json };
        }
    }
}
