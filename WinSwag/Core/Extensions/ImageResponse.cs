using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WinSwag.Core.Extensions
{
    public class ImageResponse : IResponseContent
    {
        public HttpResponseMessage Message { get; private set; }

        public ImageSource Image { get; private set; }

        private string MediaType => Message.Content.Headers.ContentType.MediaType;

        public async Task InitAsync(HttpResponseMessage message)
        {
            Message = message;
            var image = new BitmapImage();
            var stream = await message.Content.ReadAsStreamAsync();
            await image.SetSourceAsync(stream.AsRandomAccessStream());
            Image = image;
        }

        public async void CopyToClipboard()
        {
            var content = await GetResponseContentAsync();
            await DataTransferHelper.CopyToClipboardAsync(content.stream, content.contentType);
        }

        public async void SaveAsFile()
        {
            var picker = new FileSavePicker();

            switch (MediaType)
            {
                case "image/jpeg": picker.FileTypeChoices.Add("Image", new[] { ".jpg" }); break;
                case "image/png": picker.FileTypeChoices.Add("Image", new[] { ".png" }); break;
                case "image/bmp": picker.FileTypeChoices.Add("Image", new[] { ".bmp" }); break;
                case "image/svg+xml": picker.FileTypeChoices.Add("Image", new[] { ".svg" }); break;
                case "image/tiff": picker.FileTypeChoices.Add("Image", new[] { ".tif" }); break;
                case "image/webp": picker.FileTypeChoices.Add("Image", new[] { ".webp" }); break;
                default: picker.FileTypeChoices.Add("Image", new[] { ".jpg" }); break;
            }

            var file = await picker.PickSaveFileAsync();

            if (file != null)
            {
                using (var stream = await file.OpenStreamForWriteAsync())
                    await Message.Content.CopyToAsync(stream);
            }
        }

        public async void ShareFile()
        {
            var content = await GetResponseContentAsync();
            DataTransferHelper.Share(content.stream, content.contentType);
        }

        public async void OpenFile()
        {
            var content = await GetResponseContentAsync();
            await DataTransferHelper.OpenAsync(content.stream, content.contentType);
        }

        public async void OpenFileWith()
        {
            var content = await GetResponseContentAsync();
            await DataTransferHelper.OpenWithAsync(content.stream, content.contentType);
        }

        public async Task<(IRandomAccessStream stream, string contentType)> GetResponseContentAsync() =>
            ((await Message.Content.ReadAsStreamAsync()).AsRandomAccessStream(), MediaType);
    }
}
