using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using WinSwag.Core;

namespace WinSwag.Services
{
    public class SwaggerSessionManager
    {
        private const string SessionFolderName = "StoredSessions";

        private readonly ApplicationInfo _appInfo;

        private Task _initTask;
        private StorageFolder _folder;
        private SettingsDictionary<SessionInfo> _sessions;

        public SwaggerSessionManager(ApplicationInfo appInfo)
        {
            _appInfo = appInfo;
            _sessions = new SettingsDictionary<SessionInfo>(
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
                await sessionFile.CopyAsync(_folder, $"{SampleApi.SessionInfo.UrlHash}.json");
            }
        }

        public async Task<IReadOnlyList<SessionInfo>> GetAllAsync()
        {
            await _initTask;
            return _sessions.Values.OrderBy(s => s.DisplayName).ToList();
        }

        public async Task<Session> LoadAsync(string url)
        {
            await _initTask;

            if (!_sessions.TryGetValue(url, out var info))
                throw new ArgumentException($"No stored session exists for URL '{url}'");

            try
            {
                var sessionFile = await _folder.GetFileAsync($"{info.UrlHash.ToString()}.json");
                var json = await FileIO.ReadTextAsync(sessionFile);

                if (string.IsNullOrEmpty(json))
                    return EmptySession();

                return Session.FromJson(json);
            }
            catch (FileNotFoundException)
            {
                // In case someone deleted the session file, just load the session without
                // filling in any parameter values
                return EmptySession();
            }

            Session EmptySession() => new Session
            {
                Info = new SessionInfo(info.DisplayName, url),
            };
        }

        public async Task StoreAsync(Session session)
        {
            await _initTask;

            if (!_sessions.TryGetValue(session.Info.Url, out var info))
            {
                info = session.Info;
                _sessions.Add(session.Info.Url, session.Info);
            }

            var sessionFile = await _folder.CreateFileAsync($"{info.UrlHash}.json", CreationCollisionOption.ReplaceExisting);
            var json = session.ToJson();
            await FileIO.WriteTextAsync(sessionFile, json);
        }

        public async Task DeleteAsync(string url)
        {
            if (_sessions.Remove(url, out var info))
            {
                try
                {
                    var file = await _folder.GetFileAsync($"{info.UrlHash}.json");
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
        public static readonly SessionInfo SessionInfo =
            new SessionInfo("WinSwag Pinboard (Sample API)", "http://winswagsampleapi.azurewebsites.net/swagger/v1/swagger.json");

        public static readonly Uri SessionFileUri =
            new Uri("ms-appx:///Assets/SampleSessions/WinSwagPinboard.json");
    }
}
