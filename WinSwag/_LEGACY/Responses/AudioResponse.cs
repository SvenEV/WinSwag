using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace WinSwag.Core.Responses
{
    public class AudioResponse : ResponseViewModel
    {
        public IRandomAccessStream Stream { get; private set; }

        public string MimeType { get; private set; }

        public AudioResponse(HttpResponseMessage response, string requestUri) : base(response, requestUri) { }

        public static new async Task<AudioResponse> FromResponseAsync(HttpResponseMessage response, string requestUri)
        {
            var image = new BitmapImage();
            var stream = await response.Content.ReadAsStreamAsync();

            return new AudioResponse(response, requestUri)
            {
                Stream = stream.AsRandomAccessStream(),
                MimeType = response.Content.Headers.ContentType.MediaType
            };
        }
    }
}
