using System;
using System.Collections.Generic;
using System.Linq;
using WinSwag.Core;
using WinSwag.Xaml;

namespace WinSwag.ViewModels.ForModels
{
    public class EnumArgumentViewModel : ArgumentViewModelBase, IViewModel<EnumArgument>
    {
        private NamedValue _selectedOption;

        public IReadOnlyList<NamedValue> Options { get; private set; }

        public NamedValue SelectedOption
        {
            get => _selectedOption;
            set
            {
                if (Set(ref _selectedOption, value))
                    ((EnumArgument)Model).Value = _selectedOption.Value;
            }
        }

        public EnumArgumentViewModel(EnumArgument argument) : base(argument)
        {
            Options = argument.Options
                .Select(option => new NamedValue(
                    option.Key, option.Value,
                    Equals(option.Value, argument.Parameter.DefaultValue)))
                .ToList();

            _selectedOption = Options.FirstOrDefault(o => Equals(o.Value, argument.Value));
        }
    }

    public class NamedValue : IEquatable<NamedValue>
    {
        public string Name { get; }
        public object Value { get; }
        public bool IsDefault { get; }

        public NamedValue(string name, object value, bool isDefault = false)
        {
            Name = name;
            Value = value;
            IsDefault = isDefault;
        }

        public override bool Equals(object obj) => obj is NamedValue other && Equals(other);

        public override int GetHashCode() => (Name, Value, IsDefault).GetHashCode();

        public bool Equals(NamedValue other) => Equals(
            (Name, Value, IsDefault),
            (other.Name, other.Value, other.IsDefault));
    }
}
