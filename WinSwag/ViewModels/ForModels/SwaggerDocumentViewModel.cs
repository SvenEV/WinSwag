using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinSwag.ViewModels
{
    public class SwaggerDocumentViewModel
    {
        public SwaggerDocument Model { get; }

        public string Url { get; }

        public string DisplayName { get; set; }

        public IReadOnlyList<IGrouping<string, SwaggerOperationViewModel>> OperationGroups { get; }

        public SwaggerDocumentViewModel(SwaggerDocument model, string url, string displayName = null)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            DisplayName = displayName;

            OperationGroups = model.Operations
                .OrderBy(op => op.Path)
                .Select(op => new SwaggerOperationViewModel(op, Model.BaseUrl))
                .GroupBy(op => op.Model.Operation.Tags.FirstOrDefault() ?? "(Default)")
                .OrderBy(group => group.Key)
                .ToList();
        }
    }
}
