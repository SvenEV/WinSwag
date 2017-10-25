using NSwag;
using System;

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
        public SwaggerDocument Specification { get; }

        public SpecificationLoaded(SwaggerDocument specification)
        {
            Specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }
    }
}
