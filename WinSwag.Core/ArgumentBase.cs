using GalaSoft.MvvmLight;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WinSwag.Core
{
    public abstract class ArgumentBase : ObservableObject, IArgument
    {
        private ImmutableList<Parameter> _parameters;
        private string _contentType = "text/plain";
        private bool _isActive = true;

        /// <summary>
        /// For local arguments, this is the corresponding parameter.
        /// For global arguments, this is the first of the corresponding parameters.
        /// </summary>
        public Parameter Parameter => _parameters.First();

        public bool IsActive
        {
            get => _isActive;
            set => Set(ref _isActive, value);
        }

        public abstract object ObjectValue { get; set; }

        public abstract bool HasNonDefaultValue { get; }

        public string ContentType
        {
            get => _contentType;
            set => Set(ref _contentType, value);
        }

        public abstract Task ApplyAsync(HttpRequestMessage request, StringBuilder requestUri);

        public abstract JToken GetSerializedValue();

        public abstract Task SetSerializedValueAsync(JToken o);

        internal virtual IArgument Init(IEnumerable<Parameter> parameters)
        {
            _parameters = parameters?.ToImmutableList() ?? throw new ArgumentNullException(nameof(parameters));
            return this;
        }
    }
}
