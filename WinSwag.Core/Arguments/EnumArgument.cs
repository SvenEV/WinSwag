using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public class EnumArgument : ArgumentBase
    {
        private static readonly JsonSerializer _serializer = new JsonSerializer { TypeNameHandling = TypeNameHandling.All };

        private object _value;

        public IReadOnlyList<KeyValuePair<string, object>> Options { get; private set; }

        public object Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public override object ObjectValue
        {
            get => Value;
            set => Value = (string)value;
        }

        // The initial enum value is either the default value given by the spec or the first of the enum values.
        public override object InitialValue => Parameter.Specification.Default ?? Options.FirstOrDefault().Value;

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri) =>
            StringArgument.ApplyAsync(Parameter.Specification, _value?.ToString(), request, requestUri, ContentType);

        internal override IArgument Init(IEnumerable<Parameter> parameters)
        {
            base.Init(parameters);

            var parameter = parameters.First().Specification;

            if (parameter.ActualSchema.Enumeration.Contains(null))
                throw new NotSupportedException("'null' is not a valid enumeration value");

            _value = parameter.Default ?? parameter.ActualSchema.Enumeration.FirstOrDefault();

            if (parameter.ActualSchema.EnumerationNames.Count == 0)
            {
                Options = parameter.ActualSchema.Enumeration
                    .Select(v => new KeyValuePair<string, object>(v.ToString(), v))
                    .ToList();
            }
            else
            {
                Options = parameter.ActualSchema.EnumerationNames
                    .Zip(parameter.ActualSchema.Enumeration, (k, v) => new KeyValuePair<string, object>(k, v))
                    .ToList();
            }

            return this;
        }

        public override JToken GetSerializedValue()
        {
            if (Value == null)
                return null;

            return JToken.FromObject(Value, _serializer);
        }

        public override Task SetSerializedValueAsync(JToken o)
        {
            var defaultOption = Parameter.Specification.Default ?? Options.FirstOrDefault().Value;
            Value = o?.ToObject<object>(_serializer) ?? defaultOption;
            return Task.CompletedTask;
        }
    }
}
