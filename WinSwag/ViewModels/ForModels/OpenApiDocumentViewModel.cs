using System;
using WinSwag.Core;
using WinSwag.Xaml;

namespace WinSwag.ViewModels
{
    public class OpenApiDocumentViewModel : IViewModel<OpenApiDocument>
    {
        public OpenApiDocument Model { get; }

        public string DisplayName { get; set; }

        public bool HasDescription => !string.IsNullOrWhiteSpace(Model.Description);
        
        public bool HasGlobalArguments => Model.GlobalArguments?.Count > 0;

        public OpenApiDocumentViewModel(OpenApiDocument model, string displayName = null)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            DisplayName = displayName ?? model.Specification.Info.Title;
        }
    }
}
