using System;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.System;

namespace WinSwag.Services
{
    public class ApplicationInfo
    {
        private const string FirstTimeAppStartKey = "AppStartedOnce";

        public string Version
        {
            get
            {
                var v = Package.Current.Id.Version;
                return $"v{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }

        public string PackageFamilyName => Package.Current.Id.FamilyName;

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
    }
}
