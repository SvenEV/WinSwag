using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WinSwag.ViewModels
{
    public class SwaggerSpecificationViewModel
    {
        public SwaggerDocument Model { get; }

        public IReadOnlyList<IGrouping<string, OperationViewModel>> OperationGroups { get; }

        public SwaggerSpecificationViewModel(SwaggerDocument model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));

            OperationGroups = model.Operations
                .OrderBy(op => op.Path)
                .Select(op => new OperationViewModel(op, Model.BaseUrl))
                .GroupBy(op => op.Model.Operation.Tags.FirstOrDefault() ?? "(Default)")
                .OrderBy(group => group.Key)
                .ToList();
        }
    }
}
