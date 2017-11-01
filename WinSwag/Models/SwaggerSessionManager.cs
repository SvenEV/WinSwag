using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace WinSwag.Models
{
    public static class SwaggerSessionManager
    {
        private const string IndexFileName = "StoredSessions.json";
        private const string SessionFolderName = "StoredSessions";

        private static Task _initTask;
        private static StorageFolder _folder;
        private static StorageFile _indexFile;
        private static List<SwaggerSessionInfo> _sessionIndex;

        static SwaggerSessionManager() => _initTask = Init();

        private static async Task Init()
        {
            _folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(SessionFolderName, CreationCollisionOption.OpenIfExists);
            _indexFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(IndexFileName, CreationCollisionOption.OpenIfExists);
            await _initTask;
        }

        private static async Task LoadIndexFileAsync()
        {
            try
            {
                var json = await FileIO.ReadTextAsync(_indexFile);
                _sessionIndex = JsonConvert.DeserializeObject<List<SwaggerSessionInfo>>(json);
            }
            catch
            {
                _sessionIndex = new List<SwaggerSessionInfo>();
            }
        }

        private static async Task SaveIndexFileAsync()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_sessionIndex);
                await FileIO.WriteTextAsync(_indexFile, json);
            }
            catch
            {
            }
        }

        public static async Task<IReadOnlyList<SwaggerSessionInfo>> GetAllAsync()
        {
            await _initTask;
            return _sessionIndex;
        }

        public static async Task<SwaggerSession> LoadAsync(Guid sessionGuid)
        {
            await _initTask;
            var sessionFile = await _folder.GetFileAsync($"{sessionGuid}.json");
            var json = await FileIO.ReadTextAsync(sessionFile);
            var session = JsonConvert.DeserializeObject<SwaggerSession>(json);
            session.Id = sessionGuid;
            return session;
        }

        public static async Task StoreAsync(SwaggerSession session)
        {
            await _initTask;
            var sessionFile = await _folder.CreateFileAsync($"{session.Id}.json", CreationCollisionOption.ReplaceExisting);
            var json = JsonConvert.SerializeObject(session);
            await FileIO.WriteTextAsync(sessionFile, json);
        }
    }
}
