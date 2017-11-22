using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Storage;
using WinSwag.Core;
using WinSwag.ViewModels;

namespace WinSwag.Services
{
    public interface ISessionManagerVM : INotifyPropertyChanged
    {
        IReadOnlyList<SessionInfo> StoredSessions { get; }

        OpenApiDocument CurrentDocument { get; }

        bool IsSessionLoaded { get; }

        bool IsCurrentSessionFavorite { get; }

        bool IsntCurrentSessionFavorite { get; }

        Task CreateSessionAsync(string url);

        Task CreateSessionAsync(StorageFile file);

        Task DeleteSessionAsync(SessionInfo sessionInfo);

        Task LoadSessionAsync(SessionInfo sessionInfo);

        Task SaveCurrentSessionAsync(string displayName);

        Task DeleteCurrentSessionAsync();

        Task UnloadCurrentSessionAsync();
    }
}
