using Windows.ApplicationModel;

namespace WinSwag.Services
{
    public class ApplicationInfo
    {
        public string Version
        {
            get
            {
                var v = Package.Current.Id.Version;
                return $"v{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }
    }
}
