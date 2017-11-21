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

        // An enum value is "default" if
        // (a) a default option is specified and the selected option equals that default option -or-
        // (b) a default option is NOT specified and the selected option is the first option
        public override bool HasNonDefaultValue => !Equals(_value,
(object)(Parameter.Specification.Default ?? Options.FirstOrDefault().Value));

        public override Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri) =>
            StringArgument.ApplyAsync((NSwag.SwaggerParameter)Parameter.Specification, _value?.ToString(), request, requestUri, ContentType);

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
