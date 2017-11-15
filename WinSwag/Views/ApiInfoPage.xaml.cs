using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using WinSwag.Services;
using WinSwag.ViewModels;

namespace WinSwag.Views
{
    public sealed partial class ApiInfoPage : Page
    {
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register(nameof(Document), typeof(int), typeof(ApiInfoPage), new PropertyMetadata(null));

        public SwaggerDocumentViewModel Document
        {
            get { return (SwaggerDocumentViewModel)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        [Inject]
        public ISessionManagerVM SessionManagerVM { get; private set; }

        [Inject]
        public IViewStateManagerVM ViewStateManagerVM { get; private set; }

        public ApiInfoPage()
        {
            ApplicationInstance.Current.Services.Populate(this);
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Document = e.Parameter as SwaggerDocumentViewModel ?? throw new ArgumentNullException();
        }

        private async void AddToFavoritesButtonClick(object sender, RoutedEventArgs e)
        {
            var displayName = CurrentDocumentDisplayNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(displayName))
            {
                await ViewStateManagerVM.ShowMessageAsync("Please enter a name for this API.", "Add to Favorites");
                return;
            }

            await SessionManagerVM.SaveCurrentSessionAsync(displayName);
            CurrentDocumentDisplayNameTextBox.Text = "";
            AddToFavoritesFlyout.Hide();
        }

        private async void RemoveFromFavoritesButtonClick(object sender, RoutedEventArgs e)
        {
            await SessionManagerVM.DeleteCurrentSessionAsync();
            RemoveFromFavoritesFlyout.Hide();
        }

        private void CurrentDocumentDisplayNameTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                AddToFavoritesButtonClick(sender, null);
        }

        private void CurrentDocumentDisplayNameTextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            CurrentDocumentDisplayNameTextBox.SelectAll();
        }
    }
}
