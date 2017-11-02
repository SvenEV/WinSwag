using GalaSoft.MvvmLight;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml.Media;
using WinSwag.Models;
using WinSwag.Models.Arguments;

namespace WinSwag.ViewModels
{
    public class OperationViewModel : ObservableObject
    {
        public static readonly SolidColorBrush GetBrush = new SolidColorBrush(Colors.SkyBlue);
        public static readonly SolidColorBrush PostBrush = new SolidColorBrush(Colors.YellowGreen);
        public static readonly SolidColorBrush PutBrush = new SolidColorBrush(Colors.Gold);
        public static readonly SolidColorBrush DeleteBrush = new SolidColorBrush(Colors.OrangeRed);
        public static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.Gray);

        private readonly string _baseUrl;
        private ResponseViewModel _response;
        private bool _canSendRequest = true;
        private string _selectedContentType;

        public SwaggerOperationDescription Model { get; }

        public bool HasDescription => !string.IsNullOrWhiteSpace(Model.Operation.Description);

        public string Method => Model.Method.ToString().ToUpper();

        public Brush MethodBrush
        {
            get
            {
                switch (Model.Method)
                {
                    case SwaggerOperationMethod.Get: return GetBrush;
                    case SwaggerOperationMethod.Post: return PostBrush;
                    case SwaggerOperationMethod.Put: return PutBrush;
                    case SwaggerOperationMethod.Delete: return DeleteBrush;
                    default: return DefaultBrush;
                }
            }
        }

        public IReadOnlyList<SwaggerArgument> Arguments { get; }

        public ResponseViewModel Response
        {
            get => _response;
            private set
            {
                if (Set(ref _response, value))
                    RaisePropertyChanged(nameof(HasResponse));
            }
        }

        public bool HasResponse => Response != null;

        public string SelectedContentType
        {
            get => _selectedContentType;
            set => Set(ref _selectedContentType, value);
        }

        public bool CanSendRequest
        {
            get => _canSendRequest;
            private set
            {
                if (Set(ref _canSendRequest, value))
                    RaisePropertyChanged(nameof(IsBusy));
            }
        }

        public bool IsBusy => !CanSendRequest;

        public OperationViewModel(SwaggerOperationDescription model, string baseUrl)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            _baseUrl = baseUrl ?? "";
            Arguments = model.Operation.Parameters.Select(p => SwaggerArgument.FromParameter(p)).ToList();
            _selectedContentType = model.Operation.ActualConsumes?.FirstOrDefault() ?? "application/json";
        }

        public async void BeginSendRequest()
        {
            if (!_canSendRequest)
                return;

            CanSendRequest = false;

            using (var http = new HttpClient())
            {
                var requestUri = new StringBuilder(_baseUrl + Model.Path + '?');

                var request = new HttpRequestMessage
                {
                    Method = Model.Method.ToHttpMethod(),
                };

                foreach (var parameter in Arguments)
                    await parameter.ApplyAsync(request, requestUri);

                if (request.Content != null)
                    request.Content.Headers.ContentType.MediaType = _selectedContentType;

                requestUri.Length--; // remove trailing '?' or '&'
                var finalUri = requestUri.ToString();
                request.RequestUri = new Uri(finalUri);

                var response = await http.SendAsync(request);
                Response = await ResponseViewModel.FromResponseAsync(response, finalUri);
            }

            CanSendRequest = true;
        }
    }
}
