using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using WinSwag.Models;

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

        public async Task LoadAsync()
        {
            MessengerInstance.Send(AppMessage.CloseDashboard);
            MessengerInstance.Send(AppMessage.ClearCurrentSpecification);
            MessengerInstance.Send(AppMessage.BeginLoad);
            var spec = await SwaggerSpecification.LoadAsync(_url);
            MessengerInstance.Send(AppMessage.EndLoad);
            MessengerInstance.Send(new SpecificationLoaded(spec));
        }

        public async Task LoadFromQuerySubmissionAsync(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Url = args.QueryText;
            await LoadAsync();
        }

        public async Task LoadFavoriteAsync(object sender, ItemClickEventArgs e)
        {
            var favorite = (ApiFavorite)e.ClickedItem;
            Url = favorite.Url;
            await LoadAsync();
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