using System.Net.Http;
using System.Threading.Tasks;

namespace WinSwag.Core.Responses
{
    public class StringResponse : ResponseViewModel
    {
        public string Value { get; private set; }

        public StringResponse(HttpResponseMessage response, string requestUri) : base(response, requestUri) { }

        public static new async Task<StringResponse> FromResponseAsync(HttpResponseMessage response, string requestUri)
        {
            return new StringResponse(response, requestUri) { Value = await response.Content.ReadAsStringAsync() };
        }
    }
}
