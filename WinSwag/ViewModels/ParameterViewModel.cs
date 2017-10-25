using GalaSoft.MvvmLight;
using System;
using WinSwag.Models;

namespace WinSwag.ViewModels
{
    public class ParameterViewModel : ObservableObject
    {
        private string _value;

        public Parameter Model { get; }

        public string DisplayName => Model.Name + (Model.IsRequired ? "*" : "");

        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public ParameterViewModel(Parameter model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }
    }
}
