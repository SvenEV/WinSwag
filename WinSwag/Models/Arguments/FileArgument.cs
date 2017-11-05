using Newtonsoft.Json.Linq;
using NSwag;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace WinSwag.Models.Arguments
{
    public class FileArgument : SwaggerArgument
    {
        private StorageFile _file;

        public StorageFile File
        {
            get => _file;
            private set => Set(ref _file, value);
        }

        public FileArgument(SwaggerParameter parameter) : base(parameter)
        {
        }

        public async void PickFile()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add("*");
            File = await picker.PickSingleFileAsync() ?? File;
        }

        public void ClearFile()
        {
            File = null;
        }

        public override async Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri)
        {
            if (_file == null)
                return;

            if (Parameter.Kind != SwaggerParameterKind.Body)
                throw new NotSupportedException("Files can only appear as request body");

            var stream = await _file.OpenReadAsync();
            request.Content = new StreamContent(stream.AsStream());
        }

        public override JToken GetSerializedValue() => File == null ? null : JToken.FromObject(File.Path);

        public override async Task SetSerializedValueAsync(JToken o)
        {
            if (o == null)
                return;

            var path = o.ToObject<string>();
            File = path == null ? null : await StorageFile.GetFileFromPathAsync(path);
        }
    }
}
