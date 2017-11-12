using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using NSwag;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using WinSwag.Models;
using WinSwag.Services;

namespace WinSwag.ViewModels
{
    class SessionManagerVM : ViewModelBase, ISessionManagerVM
    {
        private readonly IMessenger _messenger;
        private readonly IOperationManagerVM _operationManager;
        private readonly IViewStateManagerVM _viewStateManager;
        private readonly SwaggerSessionManager _sessionManager;

        private ObservableCollection<SwaggerSessionInfo> _storedSessions = new ObservableCollection<SwaggerSessionInfo>();
        private SwaggerDocumentViewModel _currentDocument;
        private Task _initTask;

        public IReadOnlyList<SwaggerSessionInfo> StoredSessions => _storedSessions;

        public SwaggerDocumentViewModel CurrentDocument
        {
            get => _currentDocument;
            private set
            {
                if (Set(ref _currentDocument, value))
                {
                    RaisePropertyChanged(nameof(IsSessionLoaded));
                    RaisePropertyChanged(nameof(IsCurrentSessionFavorite));
                    RaisePropertyChanged(nameof(IsntCurrentSessionFavorite));
                }
            }
        }

        public bool IsSessionLoaded => CurrentDocument != null;

        public bool IsCurrentSessionFavorite => _storedSessions.Any(s => s.Url == _currentDocument?.Url);

        public bool IsntCurrentSessionFavorite => !IsCurrentSessionFavorite && CurrentDocument != null;

        public SessionManagerVM(IMessenger messenger, IOperationManagerVM operationManager,
            IViewStateManagerVM viewStateManager, SwaggerSessionManager sessionManager)
        {
            _messenger = messenger;
            _operationManager = operationManager;
            _viewStateManager = viewStateManager;
            _sessionManager = sessionManager;
            _initTask = Init();

            async Task Init()
            {
                await RefreshStoredSessionsAsync();
            }
        }

        public async Task CreateSessionAsync(string url)
        {
            try
            {
                var storedSession = _storedSessions.FirstOrDefault(s => s.Url == url);

                if (storedSession != null)
                {
                    // If the desired URL is one of the favorites, load it with all the stored arguments!
                    // (instead of creating a blank session, overwriting the stored session later on)
                    await LoadSessionAsync(storedSession);
                    return;
                }

                using (_viewStateManager.BeginTask("Creating Session..."))
                {
                    await _initTask;
                    _messenger.Send(CloseDashboard.Instance);
                    await UnloadCurrentSessionAsync();
                    var doc = await SwaggerDocument.FromUrlAsync(url);
                    var displayName = _storedSessions.FirstOrDefault(s => s.Url == url)?.DisplayName;
                    CurrentDocument = new SwaggerDocumentViewModel(doc, url, displayName);
                    _operationManager.SelectedOperation = null;
                }
            }
            catch (Exception e)
            {
                await _viewStateManager.ShowMessageAsync(e.ToString(), "Failed to Create Session");
            }
        }

        public async Task CreateSessionAsync(StorageFile file)
        {
            try
            {
                using (_viewStateManager.BeginTask("Creating Session..."))
                {
                    await _initTask;
                    _messenger.Send(CloseDashboard.Instance);
                    await UnloadCurrentSessionAsync();
                    var json = await FileIO.ReadTextAsync(file);
                    var doc = await SwaggerDocument.FromJsonAsync(json);
                    CurrentDocument = new SwaggerDocumentViewModel(doc, "file://" + file.Path);
                    _operationManager.SelectedOperation = null;
                }
            }
            catch (Exception e)
            {
                await _viewStateManager.ShowMessageAsync(e.ToString(), "Failed to Create Session");
            }
        }

        public async Task DeleteSessionAsync(SwaggerSessionInfo sessionInfo)
        {
            try
            {
                await _initTask;
                await _sessionManager.DeleteAsync(sessionInfo.Url);
                await RefreshStoredSessionsAsync();
            }
            catch (Exception e)
            {
                await _viewStateManager.ShowMessageAsync(e.ToString(), "Failed to Delete Session");
            }
        }

        public async Task LoadSessionAsync(SwaggerSessionInfo sessionInfo)
        {
            try
            {
                using (_viewStateManager.BeginTask("Loading Session..."))
                {
                    await _initTask;
                    _messenger.Send(CloseDashboard.Instance);
                    await UnloadCurrentSessionAsync(); // unload before loading new session (important when loading the same session again)
                    var session = await _sessionManager.LoadAsync(sessionInfo.Url);
                    CurrentDocument = await SwaggerSession.ToViewModelAsync(session);
                    _operationManager.SelectedOperation = null;
                }
            }
            catch (Exception e)
            {
                await _viewStateManager.ShowMessageAsync(e.ToString(), "Failed to Load Session");
            }
        }

        public async Task SaveCurrentSessionAsync(string displayName)
        {
            using (_viewStateManager.BeginTask("Saving Session..."))
            {
                await _initTask;
                CurrentDocument.DisplayName = displayName;
                var session = SwaggerSession.FromViewModel(CurrentDocument, displayName);
                await _sessionManager.StoreAsync(session);
                await RefreshStoredSessionsAsync();
            }
        }

        public async Task DeleteCurrentSessionAsync()
        {
            try
            {
                await _initTask;
                await _sessionManager.DeleteAsync(_currentDocument.Url);
                await RefreshStoredSessionsAsync();
            }
            catch (Exception e)
            {
                await _viewStateManager.ShowMessageAsync(e.ToString(), "Failed to Delete Session");
            }
        }
        
        public async Task UnloadCurrentSessionAsync()
        {
            if (_storedSessions.Any(s => s.Url == _currentDocument?.Url))
            {
                // Store current session before switching to another one
                await SaveCurrentSessionAsync(_currentDocument.DisplayName);
            }
        }

        private async Task RefreshStoredSessionsAsync()
        {
            _storedSessions.Clear();
            foreach (var info in await _sessionManager.GetAllAsync())
                _storedSessions.Add(info);

            RaisePropertyChanged(nameof(IsCurrentSessionFavorite));
            RaisePropertyChanged(nameof(IsntCurrentSessionFavorite));
        }
    }
}
