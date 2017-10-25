using GalaSoft.MvvmLight.Messaging;
using Windows.ApplicationModel.Core;
using Windows.UI;
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

        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            Window.Current.SetTitleBar(TitleBar);
            CoreApplication.MainView.TitleBar.ExtendViewIntoTitleBar = true;

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            Messenger.Default.Register<AppMessage>(this, OnAppMessage);
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
