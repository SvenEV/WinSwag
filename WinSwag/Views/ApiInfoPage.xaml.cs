using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using WinSwag.Core;
using WinSwag.Services;
using WinSwag.ViewModels;
using WinSwag.Xaml;

namespace WinSwag.Views
{
    public sealed partial class ApiInfoPage : Page
    {
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register(nameof(Document), typeof(int), typeof(ApiInfoPage), new PropertyMetadata(null));

        public OpenApiDocument Document
        {
            get => (OpenApiDocument)GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
        }

        public OpenApiDocumentViewModel DocumentVM =>
            ViewModelRegistry.ViewModelFor<OpenApiDocumentViewModel>(Document);

        [Inject]
        public ISessionManagerVM SessionManagerVM { get; private set; }

        [Inject]
        public IViewStateManagerVM ViewStateManagerVM { get; private set; }

        [Inject]
        public ApplicationInfo AppInfoVM { get; private set; }

        public ApiInfoPage()
        {
            ApplicationInstance.Current.Services.Populate(this);
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Document = e.Parameter as OpenApiDocument ?? throw new ArgumentNullException();
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

        private async void DescriptionLinkClicked(object sender, LinkClickedEventArgs e)
        {
            if (Uri.TryCreate(e.Link, UriKind.RelativeOrAbsolute, out var uri))
                await Launcher.LaunchUriAsync(uri);
        }
    }
}
