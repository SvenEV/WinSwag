using GalaSoft.MvvmLight;
using Microsoft.Extensions.DependencyInjection;
using System;
using WinSwag.Core;
using WinSwag.Services;
using WinSwag.Xaml;

namespace WinSwag.ViewModels.ForModels
{
    public class ArgumentViewModel : ArgumentViewModelBase, IViewModel<IArgument>
    {
        public ArgumentViewModel(IArgument argument) : base(argument)
        {
        }
    }

    public class ArgumentViewModelBase : ObservableObject
    {
        public IArgument Model { get; }

        public bool IsLocalArgument => Model == Model.Parameter.LocalArgument;

        public bool IsGlobalArgument => Model == Model.Parameter.GlobalArgument;

        public bool EffectiveValueIsGlobalArgument =>
            IsLocalArgument && !Model.IsActive && Model.Parameter.GlobalArgument.IsActive;

        public bool EffectiveValueIsSpecDefault =>
            Model.Parameter.DefaultValue != null &&
            ((IsLocalArgument && !Model.IsActive && !Model.Parameter.GlobalArgument.IsActive) ||
            (IsGlobalArgument && !Model.IsActive));

        public bool EffectiveValueIsNone =>
            Model.Parameter.DefaultValue == null &&
            ((IsLocalArgument && !Model.IsActive && !Model.Parameter.GlobalArgument.IsActive) ||
            (IsGlobalArgument && !Model.IsActive));

        public string InactiveText => IsLocalArgument
            ? "not included in requests"
            : "no global value assigned";

        public string ToolTip => IsLocalArgument
            ? "Check to specify a value for this parameter. " +
              "Uncheck to exclude this parameter from requests or to use the globally assigned value."
            : "Check to specify a global value that is used in operations where no value for this parameter is set. " +
              "Uncheck to not provide a global value";

        public Box<FlyoutItemClickEventHandler> GlobalArgumentRelatedOperationClickHandler { get; } =
            new Box<FlyoutItemClickEventHandler>(OnGlobalArgumentRelatedOperationClick);

        public ArgumentViewModelBase(IArgument model)
        {
            Model = model;

            model.PropertyChanges.OfProperty<bool>(nameof(IArgument.IsActive)).Subscribe(_ =>
            {
                RaisePropertyChanged(nameof(EffectiveValueIsGlobalArgument));
                RaisePropertyChanged(nameof(EffectiveValueIsSpecDefault));
                RaisePropertyChanged(nameof(EffectiveValueIsNone));
            });
        }

        private static void OnGlobalArgumentRelatedOperationClick(object sender, object clickedItem)
        {
            var parameter = (Parameter)clickedItem;
            var operationManagerVM = ApplicationInstance.Current.Services.GetService<IOperationManagerVM>();
            operationManagerVM.NavigateToOperation(parameter.Operation);
        }
    }
}
