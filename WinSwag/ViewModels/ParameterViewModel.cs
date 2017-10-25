using GalaSoft.MvvmLight;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinSwag.ViewModels
{
    public class ParameterViewModel : ObservableObject
    {
        private static readonly NamedValue NullEnumerationValue = new NamedValue("(null)", null);

        private string _value;

        public SwaggerParameter Model { get; }

        public string DisplayName => Model.Name + (Model.IsRequired ? "*" : "");

        public string SampleValue { get; }

        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public IReadOnlyList<NamedValue> EnumerationValues { get; }

        public NamedValue SelectedEnumerationValue
        {
            get => EnumerationValues.FirstOrDefault(o => o.Value?.ToString() == Value) ?? EnumerationValues.FirstOrDefault();
            set => Value = value?.Value?.ToString();
        }

        public ParameterViewModel(SwaggerParameter model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Value = model.Default?.ToString();

            if (model.ActualSchema.IsEnumeration)
            {
                if (model.ActualSchema.EnumerationNames.Count == 0)
                {
                    EnumerationValues = model.ActualSchema.Enumeration
                        .Select(v => new NamedValue(v.ToString(), v, Equals(v, model.Default)))
                        .Prepend(NullEnumerationValue)
                        .ToList();
                }
                else
                {
                    EnumerationValues = model.ActualSchema.EnumerationNames
                        .Zip(model.ActualSchema.Enumeration, (k, v) => new NamedValue(k, v, Equals(v, model.Default)))
                        .Prepend(NullEnumerationValue)
                        .ToList();
                }
            }
        }
    }

    public class NamedValue
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
    }
}
