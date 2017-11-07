using MimeTypes;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;

namespace WinSwag
{
    public static class DataTransferHelper
    {
        public static async Task<StorageFile> CreateTemporaryFileAsync(IRandomAccessStream stream, string contentType)
        {
            stream.Seek(0);
            var filename = $"{Guid.NewGuid()}{MimeTypeMap.GetExtension(contentType)}";
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

            using (var fileStream = await file.OpenStreamForWriteAsync())
                await stream.AsStream().CopyToAsync(fileStream);

            return file;
        }

        public static async Task<DataPackage> CreateDataPackageAsync(IRandomAccessStream stream, string contentType)
        {
            var file = await CreateTemporaryFileAsync(stream, contentType);

            var package = new DataPackage();
            package.Properties.Title = "WinSwag Response";
            package.SetStorageItems(new[] { file });

            if (contentType.StartsWith("image"))
                package.SetBitmap(RandomAccessStreamReference.CreateFromFile(file));

            return package;
        }

        public static async Task CopyToClipboardAsync(IRandomAccessStream stream, string contentType)
        {
            var package = await CreateDataPackageAsync(stream, contentType);
            Clipboard.SetContent(package);
            Clipboard.Flush();
        }

        public static void Share(IRandomAccessStream stream, string contentType)
        {
            var manager = DataTransferManager.GetForCurrentView();
            manager.DataRequested += OnDataRequested;
            DataTransferManager.ShowShareUI();

            async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
            {
                var deferral = args.Request.GetDeferral();
                args.Request.Data = await CreateDataPackageAsync(stream, contentType);
                deferral.Complete();
                manager.DataRequested -= OnDataRequested;
            }
        }

        public static async Task OpenAsync(IRandomAccessStream stream, string contentType)
        {
            var file = await CreateTemporaryFileAsync(stream, contentType);
            await Launcher.LaunchFileAsync(file);
        }

        public static async Task OpenWithAsync(IRandomAccessStream stream, string contentType)
        {
            var file = await CreateTemporaryFileAsync(stream, contentType);
            await Launcher.LaunchFileAsync(file, new LauncherOptions
            {
                DisplayApplicationPicker = true
            });
        }
    }
}
