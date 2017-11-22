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

namespace WinSwag.Core.Extensions
{
    public class FileArgument : ArgumentBase
    {
        private string _futureAccessToken;
        private StorageFile _value;
        private BitmapImage _thumbnail;

        public StorageFile Value
        {
            get => _value;
            set
            {
                if (Set(ref _value, value))
                    UpdateFileThumbnailAsync();
            }
        }

        public override object ObjectValue
        {
            get => Value;
            set => Value = (StorageFile)value;
        }

        public BitmapImage Thumbnail
        {
            get => _thumbnail;
            private set => Set(ref _thumbnail, value);
        }

        public override object InitialValue => null;

        public async void PickFile()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            var newFile = await picker.PickSingleFileAsync();

            if (newFile != null)
                SetFile(newFile);
        }

        public void SetFile(StorageFile file)
        {
            if (file == null)
            {
                ClearFile();
                return;
            }

            // Remove current file from FutureAccessList
            if (!string.IsNullOrEmpty(_futureAccessToken))
                StorageApplicationPermissions.FutureAccessList.Remove(_futureAccessToken);

            Value = file;

            // Add new file to FutureAccessList
            _futureAccessToken = StorageApplicationPermissions.FutureAccessList.Add(file);
        }

        public void ClearFile()
        {
            // Remove current file from FutureAccessList
            if (!string.IsNullOrEmpty(_futureAccessToken))
                StorageApplicationPermissions.FutureAccessList.Remove(_futureAccessToken);

            Value = null;
            _futureAccessToken = null;
        }

        public override async Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri)
        {
            if (_value == null)
                return;

            if (Parameter.Specification.Kind != SwaggerParameterKind.FormData)
                throw new NotSupportedException("Files can only appear as form data");

            // The following streaming approach doesn't work
            // (request just times out):
            //
            //var stream = await _file.OpenStreamForReadAsync();
            //request.Content = new StreamContent(stream);

            // Workaround: Load file into memory
            using (var stream = await _value.OpenStreamForReadAsync())
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

            if (token != null && StorageApplicationPermissions.FutureAccessList.ContainsItem(token))
            {
                Value = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token, AccessCacheOptions.DisallowUserInput);
            }
        }

        private async void UpdateFileThumbnailAsync()
        {
            if (_value == null)
            {
                Thumbnail = null;
            }
            else
            {
                try
                {
                    Thumbnail = new BitmapImage();
                    Thumbnail.SetSource(await _value.GetThumbnailAsync(ThumbnailMode.ListView));
                }
                catch
                {
                    Thumbnail = null;
                }
            }
        }
    }
}
