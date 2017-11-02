using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinSwag.ViewModels;

namespace WinSwag
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public DashboardViewModel DashboardVM { get; } = new DashboardViewModel();

        public string AppVersion
        {
            get
            {
                var v = Package.Current.Id.Version;
                return $"v{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            Window.Current.SetTitleBar(TitleBar);
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            App.CurrentMessenger.Register<AppMessage>(this, OnAppMessage);

            Window.Current.CoreWindow.KeyDown += OnKeyDown;
        }

        private void OnKeyDown(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == VirtualKey.F5)
                ViewModel.SelectedOperation.BeginSendRequest();
        }

        private void OnAppMessage(AppMessage message)
        {
            switch (message)
            {
                case AppMessage.CloseDashboard: DashboardPopup.Hide(); break;
            }
        }
    }
}
