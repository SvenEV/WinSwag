using MimeTypes;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace WinSwag.Models.Responses
{
    public class ImageResponse : ResponseViewModel
    {
        public ImageSource Image { get; private set; }

        public ImageResponse(HttpResponseMessage response, string requestUri) : base(response, requestUri) { }

        public static new async Task<ImageResponse> FromResponseAsync(HttpResponseMessage response, string requestUri)
        {
            var image = new BitmapImage();
            var stream = await response.Content.ReadAsStreamAsync();
            await image.SetSourceAsync(stream.AsRandomAccessStream());
            return new ImageResponse(response, requestUri) { Image = image };
        }

        public async void CopyToClipboard()
        {
            var stream = await Response.Content.ReadAsStreamAsync();

            var package = new DataPackage();
            package.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream.AsRandomAccessStream()));

            Clipboard.SetContent(package);
            Clipboard.Flush();
        }

        public async void SaveAsFile()
        {
            var picker = new FileSavePicker();

            switch (Response.Content.Headers.ContentType.MediaType)
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
                    await Response.Content.CopyToAsync(stream);
            }
        }

        public async void OpenFile()
        {
            var mediaType = Response.Content.Headers.ContentType.MediaType;
            var filename = $"{Guid.NewGuid()}.{MimeTypeMap.GetExtension(mediaType)}";
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (var stream = await file.OpenStreamForWriteAsync())
                await Response.Content.CopyToAsync(stream);

            await Launcher.LaunchFileAsync(file, new LauncherOptions
            {
                DisplayApplicationPicker = true
            });
        }
    }
}
