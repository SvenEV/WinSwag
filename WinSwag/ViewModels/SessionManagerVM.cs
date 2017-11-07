using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using NSwag;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ObservableCollection<SwaggerSessionInfo> _storedSessions = new ObservableCollection<SwaggerSessionInfo>();
        private SwaggerDocumentViewModel _currentDocument;
        private Task _initTask;

        public IReadOnlyList<SwaggerSessionInfo> StoredSessions => _storedSessions;

        public SwaggerDocumentViewModel CurrentDocument
        {
            get => _currentDocument;
            private set => Set(ref _currentDocument, value);
        }

        public SessionManagerVM(IMessenger messenger, IOperationManagerVM operationManager, IViewStateManagerVM viewStateManager)
        {
            _messenger = messenger;
            _operationManager = operationManager;
            _viewStateManager = viewStateManager;
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
                using (_viewStateManager.BeginTask("Creating Session..."))
                {
                    await _initTask;
                    _messenger.Send(CloseDashboard.Instance);
                    var doc = await SwaggerDocument.FromUrlAsync(url);
                    CurrentDocument = new SwaggerDocumentViewModel(doc, url);
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

        public async Task LoadSessionAsync(SwaggerSessionInfo sessionInfo)
        {
            try
            {
                using (_viewStateManager.BeginTask("Loading Session..."))
                {
                    await _initTask;
                    _messenger.Send(CloseDashboard.Instance);
                    var session = await SwaggerSessionManager.LoadAsync(sessionInfo.Url);
                    CurrentDocument = await SwaggerSession.ToViewModelAsync(session);
                    _operationManager.SelectedOperation = null;
                }
            }
            catch (Exception e)
            {
                await _viewStateManager.ShowMessageAsync(e.ToString(), "Failed to Load Session");
            }
        }

        public async Task SaveSessionAsync()
        {
            using (_viewStateManager.BeginTask("Saving Session..."))
            {
                await _initTask;
                var session = SwaggerSession.FromViewModel(CurrentDocument);
                await SwaggerSessionManager.StoreAsync(session);
                await RefreshStoredSessionsAsync();
            }
        }

        private async Task RefreshStoredSessionsAsync()
        {
            _storedSessions.Clear();
            foreach (var info in await SwaggerSessionManager.GetAllAsync())
                _storedSessions.Add(info);
        }
    }
}
