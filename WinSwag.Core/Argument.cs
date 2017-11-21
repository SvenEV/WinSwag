using NSwag;
using System;
using System.Linq;

namespace WinSwag.Core
{
    public static class Argument
    {
        public static IArgument FromSpec(SwaggerParameter spec, string defaultContentType, OpenApiSettings settings = null)
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));

            var type = settings.ArgumentTypes
                .FirstOrDefault(info => info.IsApplicable?.Invoke(spec) ?? false)?.Type ??
                settings.FallbackArgumentType;

            var argument = (IArgument)Activator.CreateInstance(type);
            argument.ContentType = defaultContentType;
            return argument;
        }
    }
}
