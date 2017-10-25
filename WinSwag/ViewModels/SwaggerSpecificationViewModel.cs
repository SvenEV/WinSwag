using System;
using System.Collections.Generic;
using System.Linq;
using WinSwag.Models;

namespace WinSwag.ViewModels
{
    public class SwaggerSpecificationViewModel
    {
        public SwaggerSpecification Model { get; }

        public IReadOnlyList<IGrouping<string, OperationViewModel>> OperationGroups { get; }

        public SwaggerSpecificationViewModel(SwaggerSpecification model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));

            OperationGroups = model.Paths.Values
                .SelectMany(path => path.Values)
                .OrderBy(op => op.Path)
                .Select(op => new OperationViewModel(op, Model.Host))
                .GroupBy(op => op.Model.Tags.FirstOrDefault() ?? "(Default)")
                .OrderBy(group => group.Key)
                .ToList();
        }
    }
}
