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
        private string _url = "";

        private readonly ObservableCollection<SwaggerSessionInfo> _favorites = new ObservableCollection<SwaggerSessionInfo>
        {
            new SwaggerSessionInfo("PetStore", "http://petstore.swagger.io/v2/swagger.json", Guid.NewGuid()),
            new SwaggerSessionInfo("HiP-DataStore", "https://docker-hip.cs.upb.de/develop/datastore/swagger/v1/swagger.json", Guid.NewGuid()),
            new SwaggerSessionInfo("HiP-UserStore", "https://docker-hip.cs.upb.de/develop/userstore/swagger/v1/swagger.json", Guid.NewGuid()),
            new SwaggerSessionInfo("HiP-CmsWebApi", "https://docker-hip.cs.upb.de/develop/cms-api/swagger/v1/swagger.json", Guid.NewGuid())
        };

        public IReadOnlyList<SwaggerSessionInfo> Favorites => _favorites;

        public string Url
        {
            get => _url;
            set => Set(ref _url, value);
        }

        public DashboardViewModel()
        {
        }

        private async Task LoadFromUrlAsync(string url)
        {
            App.CurrentMessenger.Send(AppMessage.CloseDashboard);
            App.CurrentMessenger.Send(AppMessage.ClearCurrentSpecification);
            App.CurrentMessenger.Send(AppMessage.BeginLoad);
            var spec = await SwaggerDocument.FromUrlAsync(url);
            App.CurrentMessenger.Send(AppMessage.EndLoad);
            App.CurrentMessenger.Send(new SpecificationLoaded(spec));
        }

        private async Task LoadFromFileAsync(StorageFile file)
        {
            App.CurrentMessenger.Send(AppMessage.CloseDashboard);
            App.CurrentMessenger.Send(AppMessage.ClearCurrentSpecification);
            App.CurrentMessenger.Send(AppMessage.BeginLoad);
            var json = await FileIO.ReadTextAsync(file);
            var spec = await SwaggerDocument.FromJsonAsync(json);
            App.CurrentMessenger.Send(AppMessage.EndLoad);
            App.CurrentMessenger.Send(new SpecificationLoaded(spec));
        }

        public async Task LoadFromQuerySubmissionAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Url = args.QueryText;
            await LoadFromUrlAsync(args.QueryText);
        }

        public async Task OpenFileAsync()
        {
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
            var favorite = (SwaggerSessionInfo)e.ClickedItem;
            Url = favorite.Url;
            await LoadFromUrlAsync(favorite.Url);
        }
    }
}