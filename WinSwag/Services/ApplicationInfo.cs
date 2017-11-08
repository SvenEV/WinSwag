using System;
using Windows.ApplicationModel;
using Windows.System;

namespace WinSwag.Services
{
    public class ApplicationInfo
    {
        private static readonly Uri GitHubUrl = new Uri("https://github.com/SvenEV/WinSwag");

        public string Version
        {
            get
            {
                var v = Package.Current.Id.Version;
                return $"v{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public async void OpenGitHubWebsite()
        {
            await Launcher.LaunchUriAsync(GitHubUrl);
        }
    }
}
