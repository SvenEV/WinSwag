using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinSwag.ViewModels;

namespace WinSwag.Views
{
    public sealed partial class SwaggerOperationPage : Page
    {
        public static readonly DependencyProperty OperationProperty =
            DependencyProperty.Register(nameof(Operation), typeof(SwaggerOperationViewModel), typeof(SwaggerOperationPage), new PropertyMetadata(null));

        public SwaggerOperationViewModel Operation
        {
            get { return (SwaggerOperationViewModel)GetValue(OperationProperty); }
            set { SetValue(OperationProperty, value); }
        }

        public SwaggerOperationPage()
        {
            ApplicationInstance.Current.Services.Populate(this);
            InitializeComponent();

            if (DesignMode.DesignMode2Enabled)
            {
                Operation = new SwaggerOperationViewModel(new NSwag.SwaggerOperationDescription
                {
                    Method = NSwag.SwaggerOperationMethod.Get,
                    Path = "api/Pinboard/Image",
                    Operation = new NSwag.SwaggerOperation { }
                }, "http://winswagsampleapi.azurewebsites.net");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Operation = e.Parameter as SwaggerOperationViewModel ?? throw new ArgumentNullException();
        }

        private void OnContentTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Operation.SelectedContentType = (string)e.AddedItems.FirstOrDefault();
        }
    }
}
