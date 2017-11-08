using Newtonsoft.Json;
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
            var session = JsonConvert.DeserializeObject<SwaggerSession>(json);
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
            var json = JsonConvert.SerializeObject(session);
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
}
