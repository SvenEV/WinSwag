using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using WinSwag.Core;
using WinSwag.Services;
using WinSwag.Xaml;

namespace WinSwag.ViewModels
{
    [DebuggerDisplay("{OperationId}")]
    public class OperationViewModel : ObservableObject, IViewModel<Operation>
    {
        public static readonly SolidColorBrush GetBrush = new SolidColorBrush(Colors.SkyBlue);
        public static readonly SolidColorBrush PostBrush = new SolidColorBrush(Colors.YellowGreen);
        public static readonly SolidColorBrush PutBrush = new SolidColorBrush(Colors.Gold);
        public static readonly SolidColorBrush PatchBrush = new SolidColorBrush(Colors.MediumPurple);
        public static readonly SolidColorBrush DeleteBrush = new SolidColorBrush(Colors.OrangeRed);
        public static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.Gray);

        [Inject]
        private readonly IMessenger _messenger;

        private Response _response;
        private bool _canSendRequest = true;

        public Operation Model { get; }

        public string OperationId => $"{Model.Method.ToString().ToUpper()} {Model.Path}";

        public Visibility HasDescription =>
            !string.IsNullOrWhiteSpace(Model.Description) ? Visibility.Visible : Visibility.Collapsed;

        // In the views we have bindings to 'Method' converting it to a brush that dependes on the theme.
        // If the theme changes, the binding won't update automatically. This property exists so that
        // we can manually trigger a PropertyChanged-event on 'Method' when the theme changes.
        public SwaggerOperationMethod Method => Model.Method;

        public string MethodString => Model.Method.ToString().ToUpper();

        public IEnumerable<IArgument> LocalArguments => Model.Parameters.Select(p => p.LocalArgument);

        public Response Response
        {
            get => _response;
            private set
            {
                if (Set(ref _response, value))
                    RaisePropertyChanged(nameof(HasResponse));
            }
        }

        public bool HasResponse => Response != null;

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

        public OperationViewModel(Operation model)
        {
            ApplicationInstance.Current.Services.Populate(this);
            Model = model ?? throw new ArgumentNullException(nameof(model));
            _messenger.Register<ThemeChanged>(this, _ => RaisePropertyChanged(nameof(Method)));
        }

        public async void BeginSendRequest()
        {
            if (!_canSendRequest)
                return;

            CanSendRequest = false;

            var settings = ApplicationInstance.Current.Services.GetService<ApplicationInfo>().Settings;
            Response = await OpenApi.SendRequestAsync(Model, settings);

            CanSendRequest = true;
        }
    }
}
