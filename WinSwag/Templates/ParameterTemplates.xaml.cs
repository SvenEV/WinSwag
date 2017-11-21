using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using WinSwag.Core.Extensions;

namespace WinSwag.Templates
{
    public sealed partial class ParameterTemplates : ResourceDictionary
    {
        public ParameterTemplates() => InitializeComponent();

        private async void OnFileDragOver(object sender, DragEventArgs e)
        {
            if (!e.DataView.Contains(StandardDataFormats.StorageItems))
                return;

            using (e.GetDeferral().ToDisposable())
            {
                var file = (await e.DataView.GetStorageItemsAsync()).OfType<StorageFile>().FirstOrDefault();

                if (file == null)
                    return;

                var fileArgument = (FileArgument)((FrameworkElement)sender).DataContext;
                e.AcceptedOperation = DataPackageOperation.Link;
                e.Handled = true;
            }
        }

        private async void OnFileDrop(object sender, DragEventArgs e)
        {
            using (e.GetDeferral().ToDisposable())
            {
                var fileArgument = (FileArgument)((FrameworkElement)sender).DataContext;
                var file = (await e.DataView.GetStorageItemsAsync()).OfType<StorageFile>().FirstOrDefault();
                fileArgument.SetFile(file);
            }
        }
    }
}
