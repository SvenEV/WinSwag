using GalaSoft.MvvmLight;
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

        private Response _response;
        private bool _canSendRequest = true;

        public Operation Model { get; }

        public string OperationId => $"{Model.Specification.Method.ToString().ToUpper()} {Model.Specification.Path}";

        public string Description => string.IsNullOrWhiteSpace(Model.Specification.Operation.Summary) ? Model.Specification.Operation.Description : Model.Specification.Operation.Summary;

        public Visibility HasDescription =>
            !string.IsNullOrWhiteSpace(Model.Specification.Operation.Description) ||
            !string.IsNullOrWhiteSpace(Model.Specification.Operation.Summary)
                ? Visibility.Visible : Visibility.Collapsed;

        public string Method => Model.Specification.Method.ToString().ToUpper();

        public Brush MethodBrush
        {
            get
            {
                switch (Model.Specification.Method)
                {
                    case SwaggerOperationMethod.Get: return GetBrush;
                    case SwaggerOperationMethod.Post: return PostBrush;
                    case SwaggerOperationMethod.Put: return PutBrush;
                    case SwaggerOperationMethod.Patch: return PatchBrush;
                    case SwaggerOperationMethod.Delete: return DeleteBrush;
                    default: return DefaultBrush;
                }
            }
        }

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
            Model = model ?? throw new ArgumentNullException(nameof(model));
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
