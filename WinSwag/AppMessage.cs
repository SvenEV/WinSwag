using System;
using WinSwag.Models;

namespace WinSwag
{
    public enum AppMessage
    {
        CloseDashboard,
        ClearCurrentSpecification,
        BeginLoad,
        EndLoad
    }

    public class SpecificationLoaded
    {
        public SwaggerSpecification Specification { get; }

        public SpecificationLoaded(SwaggerSpecification specification)
        {
            Specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }
    }
}
