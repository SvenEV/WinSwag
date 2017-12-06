using GalaSoft.MvvmLight.Messaging;
using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using WinSwag.Core;
using WinSwag.Services;

namespace WinSwag.Views
{
    public sealed partial class DashboardPage : Page
    {
        private CancellationTokenSource _apisGuruQueryCancellation;

        [Inject]
        private readonly IMessenger _messenger = null;

        [Inject]
        private readonly ApisGuruClient _apisGuruClient = new ApisGuruClient();

        [Inject]
        public ISessionManagerVM SessionManagerVM { get; private set; }

        [Inject]
        public ApplicationInfo AppInfoVM { get; private set; }

        public DashboardPage()
        {
            if (!DesignMode.DesignMode2Enabled)
                ApplicationInstance.Current.Services.Populate(this);

            InitializeComponent();
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
            var favorite = (SessionInfo)e.ClickedItem;
            await SessionManagerVM.LoadSessionAsync(favorite);
        }

        private async void OnUrlBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await SessionManagerVM.CreateSessionAsync(args.QueryText);
        }

        private async void OnUrlBoxQueryTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            _apisGuruQueryCancellation?.Cancel(); // cancel previous query
            _apisGuruQueryCancellation = new CancellationTokenSource();

            if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
                return;

            try
            {
                var results = await _apisGuruClient.QueryAsync(sender.Text, _apisGuruQueryCancellation.Token);

                if (results == null)
                    return; // 'null' indicates that the query was cancelled

                sender.ItemsSource = results;
            }
            catch
            {
                // Query failed (which shouldn't crash the app)
                Debugger.Break();
            }
        }

        private async void DeleteSessionButtonClick(object sender, RoutedEventArgs e)
        {
            var sessionInfo = (SessionInfo)((Button)sender).DataContext;
            await SessionManagerVM.DeleteSessionAsync(sessionInfo);
        }

        private void Close() => _messenger.Send(CloseDashboard.Instance);
    }

    public class ElementThemeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ElementTheme theme)
            {
                switch (theme)
                {
                    case ElementTheme.Default: return "System";
                    case ElementTheme.Light: return "Light";
                    case ElementTheme.Dark: return "Dark";
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotImplementedException();
    }
}
