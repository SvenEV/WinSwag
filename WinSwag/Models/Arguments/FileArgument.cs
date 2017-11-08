using Newtonsoft.Json.Linq;
using NSwag;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;

namespace WinSwag.Models.Arguments
{
    public class FileArgument : SwaggerArgument
    {
        private string _futureAccessToken;
        private StorageFile _file;
        private BitmapImage _fileThumbnail;

        public StorageFile File
        {
            get => _file;
            private set
            {
                if (Set(ref _file, value))
                    UpdateFileThumbnailAsync();
            }
        }

        public BitmapImage FileThumbnail
        {
            get => _fileThumbnail;
            private set => Set(ref _fileThumbnail, value);
        }

        public override bool HasValue => _file != null;

        public FileArgument(SwaggerParameter parameter) : base(parameter)
        {
        }

        public async void PickFile()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            var newFile = await picker.PickSingleFileAsync();

            if (newFile != null)
            {
                // Remove current file from FutureAccessList
                if (!string.IsNullOrEmpty(_futureAccessToken))
                    StorageApplicationPermissions.FutureAccessList.Remove(_futureAccessToken);

                File = newFile;

                // Add new file to FutureAccessList
                var operation = (SwaggerOperation)Parameter.Parent;
                _futureAccessToken = StorageApplicationPermissions.FutureAccessList.Add(newFile);
            }
        }

        public void ClearFile()
        {
            // Remove current file from FutureAccessList
            if (!string.IsNullOrEmpty(_futureAccessToken))
                StorageApplicationPermissions.FutureAccessList.Remove(_futureAccessToken);

            File = null;
            _futureAccessToken = null;
        }

        public override async Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri)
        {
            if (_file == null)
                return;

            if (Parameter.Kind != SwaggerParameterKind.FormData)
                throw new NotSupportedException("Files can only appear as form data");

            // The following streaming approach doesn't work
            // (request just times out):
            //
            //var stream = await _file.OpenStreamForReadAsync();
            //request.Content = new StreamContent(stream);

            // Workaround: Load file into memory
            using (var stream = await _file.OpenStreamForReadAsync())
            using (var memStream = new MemoryStream())
            {
                stream.CopyTo(memStream);
                request.Content = new ByteArrayContent(memStream.ToArray());
            }
        }

        public override JToken GetSerializedValue() => _futureAccessToken == null ? null : JToken.FromObject(_futureAccessToken);

        public override async Task SetSerializedValueAsync(JToken o)
        {
            if (o == null)
                return;

            var token = o.ToObject<string>();
            var operation = (SwaggerOperation)Parameter.Parent;

            if (token != null && StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
            {
                File = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token, AccessCacheOptions.DisallowUserInput);
            }
        }

        private async Task UpdateFileThumbnailAsync()
        {
            if (_file == null)
            {
                FileThumbnail = null;
            }
            else
            {
                FileThumbnail = new BitmapImage();
                FileThumbnail.SetSource(await _file.GetThumbnailAsync(ThumbnailMode.ListView));
            }
        }
    }
}
