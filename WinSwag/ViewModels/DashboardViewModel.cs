using GalaSoft.MvvmLight;
using NSwag;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using WinSwag.Models;

namespace WinSwag.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly Task _initTask;
        private readonly ObservableCollection<SwaggerSessionInfo> _favorites = new ObservableCollection<SwaggerSessionInfo>();
        private string _url = "";

        public IReadOnlyList<SwaggerSessionInfo> Favorites => _favorites;

        public string Url
        {
            get => _url;
            set => Set(ref _url, value);
        }

        public DashboardViewModel()
        {
            App.CurrentMessenger.Register<AppMessage>(this, msg =>
            {
                if (msg == AppMessage.StoredSessionsChanged)
                    Init();
            });

            _initTask = Init();
        }

        private async Task Init()
        {
            _favorites.Clear();
            foreach (var info in await SwaggerSessionManager.GetAllAsync())
                _favorites.Add(info);
        }

        private async Task LoadFromUrlAsync(string url)
        {
            await _initTask;
            App.CurrentMessenger.Send(AppMessage.CloseDashboard);
            App.CurrentMessenger.Send(AppMessage.ClearCurrentSpecification);
            App.CurrentMessenger.Send(AppMessage.BeginLoad);
            var spec = await SwaggerDocument.FromUrlAsync(url);
            App.CurrentMessenger.Send(AppMessage.EndLoad);
            App.CurrentMessenger.Send(new SpecificationLoaded(spec, url));
        }

        private async Task LoadFromFileAsync(StorageFile file)
        {
            await _initTask;
            App.CurrentMessenger.Send(AppMessage.CloseDashboard);
            App.CurrentMessenger.Send(AppMessage.ClearCurrentSpecification);
            App.CurrentMessenger.Send(AppMessage.BeginLoad);
            var json = await FileIO.ReadTextAsync(file);
            var spec = await SwaggerDocument.FromJsonAsync(json);
            App.CurrentMessenger.Send(AppMessage.EndLoad);
            App.CurrentMessenger.Send(new SpecificationLoaded(spec, file.Path));
        }

        public async Task LoadFromQuerySubmissionAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            await _initTask;
            Url = args.QueryText;
            await LoadFromUrlAsync(args.QueryText);
        }

        public async Task OpenFileAsync()
        {
            await _initTask;
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".json");

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                Url = "";
                await LoadFromFileAsync(file);
            }
        }

        public async Task LoadFavoriteAsync(object sender, ItemClickEventArgs e)
        {
            await _initTask;
            App.CurrentMessenger.Send(AppMessage.CloseDashboard);
            App.CurrentMessenger.Send(AppMessage.ClearCurrentSpecification);
            App.CurrentMessenger.Send(AppMessage.BeginLoad);
            var favorite = (SwaggerSessionInfo)e.ClickedItem;
            var session = await SwaggerSessionManager.LoadAsync(favorite.Url);
            App.CurrentMessenger.Send(AppMessage.EndLoad);
            App.CurrentMessenger.Send(new SessionLoaded(session));
        }
    }
}