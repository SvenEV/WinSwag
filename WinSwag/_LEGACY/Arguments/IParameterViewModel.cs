using NSwag;

namespace WinSwag.Core.Arguments
{
    public interface IParameterViewModel
    {
        SwaggerParameter Parameter { get; }
        ISwaggerArgument<object> Argument { get; }

        object DefaultValue { get; }

        string DisplayName { get; }
        bool IsBodyParameter { get; }
        bool HasDescription { get; }
    }
}
