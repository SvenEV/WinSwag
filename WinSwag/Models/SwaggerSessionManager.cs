using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using WinSwag.Services;

namespace WinSwag.Models
{
    public class SwaggerSessionManager
    {
        private const string SessionFolderName = "StoredSessions";

        private readonly ApplicationInfo _appInfo;

        private Task _initTask;
        private StorageFolder _folder;
        private SettingsDictionary<SwaggerSessionInfo> _sessions;

        public SwaggerSessionManager(ApplicationInfo appInfo)
        {
            _appInfo = appInfo;
            _sessions = new SettingsDictionary<SwaggerSessionInfo>(
                ApplicationData.Current.LocalSettings.CreateContainer("Sessions", ApplicationDataCreateDisposition.Always));
            _initTask = Init();
        }

        private async Task Init()
        {
            _folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(SessionFolderName, CreationCollisionOption.OpenIfExists);

            // Add sample session on first-time app start
            if (_appInfo.IsLaunchedFirstTime && _sessions.TryAdd(SampleApi.SessionInfo.Url, SampleApi.SessionInfo))
            {
                var sessionFile = await StorageFile.GetFileFromApplicationUriAsync(SampleApi.SessionFileUri);
                await sessionFile.CopyAsync(_folder, $"{SampleApi.SessionInfo.Guid}.json");
            }
        }

        public async Task<IReadOnlyList<SwaggerSessionInfo>> GetAllAsync()
        {
            await _initTask;
            return _sessions.Values.OrderBy(s => s.DisplayName).ToList();
        }

        public async Task<SwaggerSession> LoadAsync(string url)
        {
            await _initTask;

            if (!_sessions.TryGetValue(url, out var info))
                throw new ArgumentException($"No stored session exists for URL '{url}'");

            try
            {
                var sessionFile = await _folder.GetFileAsync($"{info.Guid.ToString()}.json");
                var json = await FileIO.ReadTextAsync(sessionFile);
                return SwaggerSession.FromJson(json);
            }
            catch (FileNotFoundException)
            {
                // In case someone deleted the session file, just load the session without
                // filling in any parameter values
                return new SwaggerSession
                {
                    Url = url,
                    DisplayName = info.DisplayName,
                    Operations = new Dictionary<string, SwaggerSession.StoredOperation>()
                };
            }
        }

        public async Task StoreAsync(SwaggerSession session)
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

        public async Task DeleteAsync(string url)
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
