using NSwag;
using System;
using System.Collections.Generic;
using System.Linq;
using WinSwag.Models.Arguments;

namespace WinSwag.ViewModels
{
    public class SwaggerDocumentViewModel
    {
        public SwaggerDocument Model { get; }

        public string Url { get; }

        public string DisplayName { get; set; }

        public string Description => Model.Info.Description?.Trim();

        public bool HasDescription => !string.IsNullOrWhiteSpace(Model.Info.Description);

        public IReadOnlyList<IGrouping<string, SwaggerOperationViewModel>> OperationGroups { get; }

        /// <summary>
        /// Parameters that occur in all operations and can thus be assigned globally.
        /// </summary>
        public IReadOnlyList<SwaggerArgument> CommonParameters { get; }

        public bool HasCommonParameters => CommonParameters?.Count > 0;

        public SwaggerDocumentViewModel(SwaggerDocument model, string url, string displayName = null)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Url = url ?? throw new ArgumentNullException(nameof(url));
            DisplayName = displayName ?? model.Info.Title;

            OperationGroups = model.Operations
                .OrderBy(op => op.Path)
                .Select(op => new SwaggerOperationViewModel(op, Model.BaseUrl))
                .GroupBy(op => op.Model.Operation.Tags.FirstOrDefault() ?? "(Default)")
                .OrderBy(group => group.Key)
                .ToList();

            CommonParameters = OperationGroups
                .SelectMany(group => group)
                .SelectMany(op => op.Arguments)
                .GroupBy(arg => arg.ParameterId)
                .Where(group => group.Count() == model.Operations.Count())
                .Select(group => SwaggerArgument.FromParameter(group.First().Parameter))
                .ToList();
        }
    }
}
