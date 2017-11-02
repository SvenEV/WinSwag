using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinSwag.Models.Responses;

namespace WinSwag.Templates
{
    public sealed partial class ResponseTemplates : ResourceDictionary
    {
        public ResponseTemplates() => InitializeComponent();

        private void OnAudioTemplateLoaded(object sender, RoutedEventArgs e)
        {
            var media = (MediaElement)sender;
            var vm = (AudioResponse)media.DataContext;
            media.SetSource(vm.Stream, vm.MimeType);
        }

        private void OnImageTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var image = (Image)sender;
            image.ContextFlyout.ShowAt(image);
        }
    }
}
