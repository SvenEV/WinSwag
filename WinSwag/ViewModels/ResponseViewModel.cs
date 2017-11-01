using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WinSwag.ViewModels
{
    public class ResponseViewModel
    {
        public HttpResponseMessage Model { get; }

        public string Status => $"{(int)Model.StatusCode} {Model.StatusCode}";

        public string Content { get; }

        public string RequestUri { get; }

        public ResponseViewModel(HttpResponseMessage model, string content, string requestUri)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            RequestUri = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
        }

        public static async Task<ResponseViewModel> FromResponseAsync(HttpResponseMessage model, string requestUri)
        {
            var content = await model.Content.ReadAsStringAsync();

            // Try formatting as indented JSON (in background thread)
            await Task.Run(() =>
            {
                try
                {
                    var o = JToken.Parse(content);
                    content = o.ToString(Formatting.Indented);
                }
                catch (JsonReaderException)
                {
                }
            });

            return new ResponseViewModel(model, content, requestUri);
        }
    }
}
