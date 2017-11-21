using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinSwag.Core;

namespace WinSwag.Templates
{
    public sealed partial class CommonResources : ResourceDictionary
    {
        public CommonResources() => InitializeComponent();

        private void OnContentTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var argument = (IArgument)((FrameworkElement)sender).DataContext;
            argument.ContentType = (string)e.AddedItems.FirstOrDefault();
        }
    }
}
