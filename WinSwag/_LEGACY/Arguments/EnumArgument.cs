using GalaSoft.MvvmLight;
using Newtonsoft.Json.Linq;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core.Arguments
{
    public class EnumArgument : ObservableObject, ISwaggerArgument<NamedValue>
    {
        private static readonly NamedValue NullEnumerationValue = new NamedValue("(null)", null);

        private NamedValue _value;

        public IReadOnlyList<NamedValue> Options { get; }

        public NamedValue Value
        {
            get => _value;
            set => Set(ref _value, value ?? NullEnumerationValue);
        }

        public bool HasValue => _value != NullEnumerationValue;

        public EnumArgument(SwaggerParameter parameter)
        {
            if (parameter.ActualSchema.EnumerationNames.Count == 0)
            {
                Options = parameter.ActualSchema.Enumeration
                    .Select(v => new NamedValue(v.ToString(), v, Equals(v, parameter.Default)))
                    .Prepend(NullEnumerationValue)
                    .ToList();
            }
            else
            {
                Options = parameter.ActualSchema.EnumerationNames
                    .Zip(parameter.ActualSchema.Enumeration, (k, v) => new NamedValue(k, v, Equals(v, parameter.Default)))
                    .Prepend(NullEnumerationValue)
                    .ToList();
            }
        }

        public Task ApplyAsync(SwaggerParameter parameter, HttpRequestMessage request, StringBuilder requestUri, string contentType) =>
            StringArgument.ApplyAsync(parameter, _value?.Value?.ToString(), request, requestUri, contentType);


        public JToken GetSerializedValue() => Value == null ? null : JToken.FromObject(Value);

        public Task SetSerializedValueAsync(JToken o) => Task.FromResult(Value = o?.ToObject<NamedValue>());
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
