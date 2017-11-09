using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinSwag.Models
{
    public static class SwaggerSessionManager
    {
        private const string SessionFolderName = "StoredSessions";
        private const string FirstTimeAppStartKey = "AppStartedOnce";

        private static Task _initTask;
        private static StorageFolder _folder;
        private static SettingsDictionary<SwaggerSessionInfo> _sessions;

        static SwaggerSessionManager()
        {
            _sessions = new SettingsDictionary<SwaggerSessionInfo>(
                ApplicationData.Current.LocalSettings.CreateContainer("Sessions", ApplicationDataCreateDisposition.Always));
            _initTask = Init();
        }

        private static async Task Init()
        {
            _folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(SessionFolderName, CreationCollisionOption.OpenIfExists);

            // Add sample session on first-time app start
            var settings = ApplicationData.Current.LocalSettings.Values;
            if (!settings.ContainsKey(FirstTimeAppStartKey) &&
                _sessions.TryAdd(SampleApi.SessionInfo.Url, SampleApi.SessionInfo))
            {
                var sessionFile = await StorageFile.GetFileFromApplicationUriAsync(SampleApi.SessionFileUri);
                await sessionFile.CopyAsync(_folder, $"{SampleApi.SessionInfo.Guid}.json");
                settings.Add(FirstTimeAppStartKey, true);
            }
        }

        public static async Task<IReadOnlyList<SwaggerSessionInfo>> GetAllAsync()
        {
            await _initTask;
            return _sessions.Values.OrderBy(s => s.DisplayName).ToList();
        }

        public static async Task<SwaggerSession> LoadAsync(string url)
        {
            await _initTask;

            if (!_sessions.TryGetValue(url, out var info))
                throw new ArgumentException($"No stored session exists for URL '{url}'");

            var sessionFile = await _folder.GetFileAsync($"{info.Guid.ToString()}.json");
            var json = await FileIO.ReadTextAsync(sessionFile);
            var session = SwaggerSession.FromJson(json);
            return session;
        }

        public static async Task StoreAsync(SwaggerSession session)
        {
            await _initTask;

            if (!_sessions.TryGetValue(session.Url, out var info))
            {
                info = new SwaggerSessionInfo(session.DisplayName, session.Url, Guid.NewGuid());
                _sessions.Add(session.Url, info);
            }

            var sessionFile = await _folder.CreateFileAsync($"{info.Guid}.json", CreationCollisionOption.ReplaceExisting);
            var json = session.ToJson();
            await FileIO.WriteTextAsync(sessionFile, json);
        }

        public static async Task DeleteAsync(string url)
        {
            if (_sessions.Remove(url, out var info))
            {
                try
                {
                    var file = await _folder.GetFileAsync($"{info.Guid}.json");
                    await file.DeleteAsync();
                }
                catch (FileNotFoundException) // If file already deleted, that's fine
                {
                }
            }
        }
    }

    static class SampleApi
    {
        public static readonly SwaggerSessionInfo SessionInfo =
            new SwaggerSessionInfo("WinSwag Pinboard (Sample API)", "http://winswagsampleapi.azurewebsites.net/swagger/v1/swagger.json", Guid.Empty);

        public static readonly Uri SessionFileUri =
            new Uri("ms-appx:///Assets/SampleSessions/WinSwagPinboard.json");
    }
}
