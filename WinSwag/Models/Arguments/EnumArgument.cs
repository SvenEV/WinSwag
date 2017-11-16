using Newtonsoft.Json.Linq;
using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Models.Arguments
{
    public class EnumArgument : SwaggerArgument
    {
        private static readonly NamedValue NullEnumerationValue = new NamedValue("(null)", null);

        private NamedValue _value;

        public IReadOnlyList<NamedValue> Options { get; }

        public NamedValue Value
        {
            get => _value;
            set => Set(ref _value, value ?? NullEnumerationValue);
        }

        public override bool HasValue => _value != NullEnumerationValue;

        public EnumArgument(SwaggerParameter parameter) : base(parameter)
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

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri, string contentType) =>
            StringArgument.ApplyAsync(Parameter, _value?.Value?.ToString(), request, requestUri, contentType);


        public override JToken GetSerializedValue() => Value == null ? null : JToken.FromObject(Value);

        public override Task SetSerializedValueAsync(JToken o) => Task.FromResult(Value = o?.ToObject<NamedValue>());
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
