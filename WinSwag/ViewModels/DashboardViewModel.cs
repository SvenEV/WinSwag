using GalaSoft.MvvmLight;
using NSwag;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

namespace WinSwag.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private string _url = "";

        private readonly ObservableCollection<ApiFavorite> _favorites = new ObservableCollection<ApiFavorite>
        {
            new ApiFavorite("PetStore", "http://petstore.swagger.io/v2/swagger.json"),
            new ApiFavorite("HiP-DataStore", "https://docker-hip.cs.upb.de/develop/datastore/swagger/v1/swagger.json"),
            new ApiFavorite("HiP-UserStore", "https://docker-hip.cs.upb.de/develop/userstore/swagger/v1/swagger.json"),
            new ApiFavorite("HiP-CmsWebApi", "https://docker-hip.cs.upb.de/develop/cms-api/swagger/v1/swagger.json")
        };

        public IReadOnlyList<ApiFavorite> Favorites => _favorites;

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
            MessengerInstance.Send(AppMessage.CloseDashboard);
            MessengerInstance.Send(AppMessage.ClearCurrentSpecification);
            MessengerInstance.Send(AppMessage.BeginLoad);
            var spec = await SwaggerDocument.FromUrlAsync(url);
            MessengerInstance.Send(AppMessage.EndLoad);
            MessengerInstance.Send(new SpecificationLoaded(spec));
        }

        private async Task LoadFromFileAsync(StorageFile file)
        {
            MessengerInstance.Send(AppMessage.CloseDashboard);
            MessengerInstance.Send(AppMessage.ClearCurrentSpecification);
            MessengerInstance.Send(AppMessage.BeginLoad);
            var json = await FileIO.ReadTextAsync(file);
            var spec = await SwaggerDocument.FromJsonAsync(json);
            MessengerInstance.Send(AppMessage.EndLoad);
            MessengerInstance.Send(new SpecificationLoaded(spec));
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
            var favorite = (ApiFavorite)e.ClickedItem;
            Url = favorite.Url;
            await LoadFromUrlAsync(favorite.Url);
        }
    }

    public class ApiFavorite
    {
        public string DisplayName { get; }

        public string Url { get; }

        public ApiFavorite(string displayName, string url)
        {
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }
}