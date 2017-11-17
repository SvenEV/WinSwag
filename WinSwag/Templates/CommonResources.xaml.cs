using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinSwag.Templates
{
    public sealed partial class CommonResources : ResourceDictionary
    {
        public CommonResources() => InitializeComponent();

        private void OnContentTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: Can't access the operation here...
            //var argument = (SwaggerArgument)((FrameworkElement)sender).DataContext;
            //Operation.SelectedContentType = (string)e.AddedItems.FirstOrDefault();
        }
    }
}
