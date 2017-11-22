using GalaSoft.MvvmLight;
using System;
using WinSwag.Core;
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

        private bool IsLocalArgument => Model == Model.Parameter.LocalArgument;

        private bool IsGlobalArgument => Model == Model.Parameter.GlobalArgument;

        public bool EffectiveValueIsGlobalArgument =>
            IsLocalArgument && !Model.IsActive && Model.Parameter.GlobalArgument.IsActive;

        public bool EffectiveValueIsSpecDefault =>
            Model.Parameter.Specification.Default != null &&
            ((IsLocalArgument && !Model.IsActive && !Model.Parameter.GlobalArgument.IsActive) ||
            (IsGlobalArgument && !Model.IsActive));

        public bool EffectiveValueIsNone =>
            Model.Parameter.Specification.Default == null &&
            ((IsLocalArgument && !Model.IsActive && !Model.Parameter.GlobalArgument.IsActive) ||
            (IsGlobalArgument && !Model.IsActive));

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
    }
}
