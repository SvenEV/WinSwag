using System;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;

namespace WinSwag.Services
{
    public class ApplicationInfo
    {
        private const string FirstTimeAppStartKey = "AppStartedOnce";
        private static readonly Uri GitHubUrl = new Uri("https://github.com/SvenEV/WinSwag");

        public string Version
        {
            get
            {
                var v = Package.Current.Id.Version;
                return $"v{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        /// <summary>
        /// Indicates whether the app was launched for the first time after being installed.
        /// </summary>
        public bool IsLaunchedFirstTime { get; }

        public ApplicationInfo()
        {
            var settings = ApplicationData.Current.LocalSettings.Values;
            IsLaunchedFirstTime = !settings.ContainsKey(FirstTimeAppStartKey);
            if (IsLaunchedFirstTime)
                settings.Add(FirstTimeAppStartKey, true);
        }

        public async void OpenGitHubWebsite()
        {
            await Launcher.LaunchUriAsync(GitHubUrl);
        }
    }
}
