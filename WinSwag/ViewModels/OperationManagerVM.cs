using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;
using WinSwag.Core;
using WinSwag.Services;

namespace WinSwag.ViewModels
{
    public class OperationManagerVM : ViewModelBase, IOperationManagerVM
    {
        private readonly IMessenger _messenger;
        private readonly IViewStateManagerVM _viewStateManager;
        private readonly List<Operation> _navigationStack = new List<Operation>();
        private int _navigationIndex = -1;

        public Operation SelectedOperation => _navigationIndex == -1 ? null : _navigationStack[_navigationIndex];

        public bool CanGoBack => _navigationIndex > 0;

        public bool CanGoForward => _navigationIndex >= 0 && _navigationIndex < _navigationStack.Count - 1;

        public bool IsOperationSelected => SelectedOperation != null;

        public bool IsntOperationSelected => SelectedOperation == null;

        public OperationManagerVM(IMessenger messenger, IViewStateManagerVM viewStateManager)
        {
            _messenger = messenger;
            _viewStateManager = viewStateManager;
        }

        public void ClearNavigationStack()
        {
            _navigationStack.Clear();
            _navigationIndex = -1;
            RaisePropertyChangeEvents();
        }

        public void NavigateToApiInfo() => NavigateToOperation(null);

        public void NavigateToOperation(Operation operation)
        {
            if (operation == SelectedOperation && _navigationIndex != -1)
                return; // Don't navigate to the same page again

            // Clear forward stack
            while (_navigationStack.Count > _navigationIndex + 1)
                _navigationStack.RemoveAt(_navigationIndex + 1);

            // Push onto stack
            _navigationStack.Add(operation);
            _navigationIndex++;

            RaisePropertyChangeEvents();
            _messenger.Send(new NavigatedToOperation(SelectedOperation, NavigationMode.New));
        }

        public void GoBack()
        {
            if (!CanGoBack)
                return;

            _navigationIndex--;
            RaisePropertyChangeEvents();
            _messenger.Send(new NavigatedToOperation(SelectedOperation, NavigationMode.Back));
        }

        public void GoForward()
        {
            if (!CanGoForward)
                return;

            _navigationIndex++;
            RaisePropertyChangeEvents();
            _messenger.Send(new NavigatedToOperation(SelectedOperation, NavigationMode.Forward));
        }

        private void RaisePropertyChangeEvents()
        {
            RaisePropertyChanged(nameof(CanGoBack));
            RaisePropertyChanged(nameof(CanGoForward));
            RaisePropertyChanged(nameof(IsOperationSelected));
            RaisePropertyChanged(nameof(IsntOperationSelected));
            RaisePropertyChanged(nameof(SelectedOperation));
        }
    }

    public class NavigatedToOperation
    {
        public Operation Operation { get; }

        public NavigationMode NavigationMode { get; }

        public NavigatedToOperation(Operation operation, NavigationMode navigationMode)
        {
            Operation = operation;
            NavigationMode = navigationMode;
        }
    }
}