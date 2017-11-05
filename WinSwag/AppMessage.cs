using NSwag;
using System;
using WinSwag.Models;

namespace WinSwag
{
    public enum AppMessage
    {
        CloseDashboard,
        ClearCurrentSpecification,
        BeginLoad,
        EndLoad,
        StoredSessionsChanged
    }

    public class SpecificationLoaded
    {
        public SwaggerDocument Specification { get; }

        public string Url { get; }

        public SpecificationLoaded(SwaggerDocument specification, string url)
        {
            Specification = specification ?? throw new ArgumentNullException(nameof(specification));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }
    }

    public class SessionLoaded
    {
        public SwaggerSession Session { get; }

        public SessionLoaded(SwaggerSession session)
        {
            Session = session ?? throw new ArgumentNullException(nameof(session));
        }
    }
}
