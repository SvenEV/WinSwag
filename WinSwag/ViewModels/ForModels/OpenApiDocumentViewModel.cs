using System;
using System.Linq;
using WinSwag.Core;
using WinSwag.Xaml;

namespace WinSwag.ViewModels
{
    public class OpenApiDocumentViewModel : IViewModel<OpenApiDocument>
    {
        public OpenApiDocument Model { get; }

        public bool HasDescription => !string.IsNullOrWhiteSpace(Model.Description);

        public bool HasGlobalArguments => Model.GlobalArguments?.Count > 0;

        public bool CanAuthorize => Model.SecuritySchemes.Any();

        public OpenApiDocumentViewModel(OpenApiDocument model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }
    }
}
