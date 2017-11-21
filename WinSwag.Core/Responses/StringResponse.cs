using System.Net.Http;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class StringResponse : IResponseContent
    {
        public string Value { get; private set; }

        public async Task InitAsync(HttpResponseMessage message)
        {
            Value = await message.Content.ReadAsStringAsync();
        }
    }
}
