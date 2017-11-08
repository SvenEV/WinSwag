using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WinSwag.Models;
using WinSwag.Services;

namespace WinSwag
{
    public sealed partial class MainPage : Page
    {
        private readonly IMessenger _messenger;

        public ApplicationInfo AppInfoVM { get; }

        public ISessionManagerVM SessionManagerVM { get; }

        public IOperationManagerVM OperationManagerVM { get; }

        public IViewStateManagerVM ViewStateManagerVM { get; }

        public MainPage()
        {
            _messenger = ApplicationInstance.Current.Services.GetService<IMessenger>();
            AppInfoVM = ApplicationInstance.Current.Services.GetService<ApplicationInfo>();
            SessionManagerVM = ApplicationInstance.Current.Services.GetService<ISessionManagerVM>();
            OperationManagerVM = ApplicationInstance.Current.Services.GetService<IOperationManagerVM>();
            ViewStateManagerVM = ApplicationInstance.Current.Services.GetService<IViewStateManagerVM>();

            InitializeComponent();

            Window.Current.SetTitleBar(TitleBar);
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            _messenger.Register<CloseDashboard>(this, _ => DashboardPopup.Hide());

            Window.Current.CoreWindow.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.F5 when OperationManagerVM.IsOperationSelected:
                    OperationManagerVM.SelectedOperation.BeginSendRequest();
                    args.Handled = true;
                    break;

                case VirtualKey.Escape when !args.Handled:
                    DashboardPopup.IsOpen = !DashboardPopup.IsOpen;
                    args.Handled = true;
                    break;
            }
        }

        private async void OpenFileButtonClick(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".json");

            var file = await picker.PickSingleFileAsync();

            if (file != null)
                await SessionManagerVM.CreateSessionAsync(file);
        }

        private async void StoredSessionClick(object sender, ItemClickEventArgs e)
        {
            var favorite = (SwaggerSessionInfo)e.ClickedItem;
            await SessionManagerVM.LoadSessionAsync(favorite);
        }

        private async void OnUrlBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await SessionManagerVM.CreateSessionAsync(args.QueryText);
        }

        private void OnContentTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OperationManagerVM.SelectedOperation != null)
                OperationManagerVM.SelectedOperation.SelectedContentType = (string)e.AddedItems.FirstOrDefault();
        }

        private async void DeleteSessionButtonClick(object sender, RoutedEventArgs e)
        {
            var sessionInfo = (SwaggerSessionInfo)((Button)sender).DataContext;
            await SessionManagerVM.DeleteSessionAsync(sessionInfo);
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

        private void OnCurrentDocumentDisplayNameTextBoxKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                AddToFavoritesButtonClick(sender, null);
        }
    }
}
