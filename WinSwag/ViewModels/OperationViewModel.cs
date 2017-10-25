using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using Windows.UI;
using Windows.UI.Xaml.Media;
using WinSwag.Models;

namespace WinSwag.ViewModels
{
    public class OperationViewModel : ObservableObject
    {
        public static readonly SolidColorBrush GetBrush = new SolidColorBrush(Colors.SkyBlue);
        public static readonly SolidColorBrush PostBrush = new SolidColorBrush(Colors.YellowGreen);
        public static readonly SolidColorBrush PutBrush = new SolidColorBrush(Colors.Gold);
        public static readonly SolidColorBrush DeleteBrush = new SolidColorBrush(Colors.OrangeRed);
        public static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.Gray);

        private readonly string _host;
        private ResponseViewModel _response;
        private bool _canSendRequest = true;

        public Operation Model { get; }

        public bool HasDescription => !string.IsNullOrWhiteSpace(Model.Description);

        public string Method => Model.Method.ToString().ToUpper();

        public Brush MethodBrush
        {
            get
            {
                switch (Model.Method)
                {
                    case HttpVerb.Get: return GetBrush;
                    case HttpVerb.Post: return PostBrush;
                    case HttpVerb.Put: return PutBrush;
                    case HttpVerb.Delete: return DeleteBrush;
                    default: return DefaultBrush;
                }
            }
        }

        public IReadOnlyList<ParameterViewModel> Parameters { get; }

        public ResponseViewModel Response
        {
            get => _response;
            private set => Set(ref _response, value);
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

        public OperationViewModel(Operation model, string host)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            _host = host ?? throw new ArgumentNullException(nameof(host));
            Parameters = model.Parameters.Select(p => new ParameterViewModel(p)).ToList();
        }

        public async void BeginSendRequest()
        {
            if (!_canSendRequest)
                return;

            CanSendRequest = false;

            using (var http = new HttpClient())
            {
                var requestUri = Model.Path;
                var isFirstQueryParameter = true;

                var request = new HttpRequestMessage
                {
                    Method = Model.Method.ToHttpMethod(),
                };

                foreach (var parameter in Parameters.Where(p => !string.IsNullOrEmpty(p.Value)))
                {
                    switch (parameter.Model.Location)
                    {
                        case ParameterLocation.Path:
                            requestUri = requestUri.Replace("{" + parameter.Model.Name + "}", parameter.Value);
                            break;

                        case ParameterLocation.Header:
                            request.Headers.Add(parameter.Model.Name, parameter.Value);
                            break;

                        case ParameterLocation.Query:
                            var value = Uri.EscapeDataString(parameter.Value);
                            requestUri += $"{(isFirstQueryParameter ? "?" : "&")}{parameter.Model.Name}={value}";
                            isFirstQueryParameter = false;
                            break;

                        case ParameterLocation.Body:
                            request.Content = new StringContent(parameter.Value, Encoding.UTF8, "application/json");
                            break;

                        // TODO
                    }
                }

                var fullRequestUri = $"https://{_host}{requestUri}";
                request.RequestUri = new Uri(fullRequestUri);

                var response = await http.SendAsync(request);
                Response = await ResponseViewModel.FromResponseAsync(response, fullRequestUri);
            }

            CanSendRequest = true;
        }
    }
}
