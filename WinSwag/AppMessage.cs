using NSwag;
using System;
using WinSwag.Models;

namespace WinSwag
{
    public class CloseDashboard
    {
        public static readonly CloseDashboard Instance = new CloseDashboard();
        private CloseDashboard() { }
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
