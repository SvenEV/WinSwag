using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            if (args.VirtualKey == VirtualKey.F5)
                OperationManagerVM.SelectedOperation.BeginSendRequest();
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
    }
}
