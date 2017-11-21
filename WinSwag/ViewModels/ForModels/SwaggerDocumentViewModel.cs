using System;
using System.Collections.Generic;
using System.Linq;
using WinSwag.Core;

namespace WinSwag.ViewModels
{
    public class SwaggerDocumentViewModel
    {
        public OpenApiDocument Model { get; }

        public string DisplayName { get; set; }

        public bool HasDescription => !string.IsNullOrWhiteSpace(Model.Description);
        
        public bool HasGlobalArguments => Model.GlobalArguments?.Count > 0;

        public SwaggerDocumentViewModel(OpenApiDocument model, string displayName = null)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            DisplayName = displayName ?? model.Specification.Info.Title;
        }
    }
}
