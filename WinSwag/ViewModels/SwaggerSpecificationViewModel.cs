using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinSwag.ViewModels
{
    public class SwaggerSpecificationViewModel
    {
        public SwaggerDocument Model { get; }

        public string Url { get; }

        public IReadOnlyList<IGrouping<string, OperationViewModel>> OperationGroups { get; }

        public SwaggerSpecificationViewModel(SwaggerDocument model, string url)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Url = url ?? throw new ArgumentNullException(nameof(url));

            OperationGroups = model.Operations
                .OrderBy(op => op.Path)
                .Select(op => new OperationViewModel(op, Model.BaseUrl))
                .GroupBy(op => op.Model.Operation.Tags.FirstOrDefault() ?? "(Default)")
                .OrderBy(group => group.Key)
                .ToList();
        }
    }
}
