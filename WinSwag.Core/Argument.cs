using NSwag;
using System;
using System.Linq;

namespace WinSwag.Core
{
    public static class Argument
    {
        public static IArgument FromSpec(SwaggerParameter spec, DocumentCreationContext context)
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var type = context.Settings.ArgumentTypes
                .FirstOrDefault(info => info.IsApplicable?.Invoke(spec) ?? false)?.Type ??
                context.Settings.FallbackArgumentType;

            var defaultContentType = context.CurrentOperation.AcceptedContentTypes.FirstOrDefault() ?? "application/json";
            var argument = (IArgument)Activator.CreateInstance(type);
            argument.ContentType = defaultContentType;
            return argument;
        }
    }
}
