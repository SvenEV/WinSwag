using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace WinSwag.Models.Responses
{
    public abstract class ResponseViewModel
    {
        public HttpResponseMessage Response { get; }

        public string Status => $"{(int)Response.StatusCode} {Response.StatusCode}";

        public string RequestUri { get; }

        public ResponseViewModel(HttpResponseMessage response, string requestUri)
        {
            Response = response ?? throw new ArgumentNullException(nameof(response));
            RequestUri = requestUri ?? throw new ArgumentNullException(nameof(requestUri));
        }

        public async void SaveAs()
        {
            var picker = new FileSavePicker();
            picker.FileTypeChoices.Add("Text file", new[] { ".txt" });
            var file = await picker.PickSaveFileAsync();

            if (file != null)
            {
                using (var stream = await file.OpenStreamForWriteAsync())
                    await Response.Content.CopyToAsync(stream);
            }
        }

        public static async Task<ResponseViewModel> FromResponseAsync(HttpResponseMessage response, string requestUri)
        {
            var mediaType = response.Content.Headers.ContentType.MediaType;

            if (mediaType.StartsWith("image/"))
                return await ImageResponse.FromResponseAsync(response, requestUri);

            if (mediaType.StartsWith("audio/"))
                return await AudioResponse.FromResponseAsync(response, requestUri);

            switch (mediaType)
            {
                case "application/json": return await JsonResponse.FromResponseAsync(response, requestUri);
                default: return await StringResponse.FromResponseAsync(response, requestUri);
            }
        }
    }
}
