using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace WinSwag.Core.Extensions
{
    public class AudioResponse : IResponseContent
    {
        public IRandomAccessStream Stream { get; private set; }

        public string MimeType { get; private set; }

        public async Task InitAsync(HttpResponseMessage message)
        {
            var image = new BitmapImage();
            var stream = await message.Content.ReadAsStreamAsync();

            Stream = stream.AsRandomAccessStream();
            MimeType = message.Content.Headers.ContentType.MediaType;
        }
    }
}
