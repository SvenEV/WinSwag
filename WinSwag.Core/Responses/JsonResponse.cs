using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class JsonResponse : IResponseContent
    {
        public string Json { get; private set; }

        public async Task InitAsync(HttpResponseMessage message)
        {
            var json = await message.Content.ReadAsStringAsync();

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

            Json = json;
        }
    }
}
