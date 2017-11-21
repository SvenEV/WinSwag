using System.ComponentModel;
using WinSwag.Core;
using WinSwag.ViewModels;

namespace WinSwag.Services
{
    public interface IOperationManagerVM : INotifyPropertyChanged
    {
        Operation SelectedOperation { get; }

        bool CanGoBack { get; }

        bool CanGoForward { get; }

        bool IsOperationSelected { get; }

        bool IsntOperationSelected { get; }

        void NavigateToApiInfo();

        void NavigateToOperation(Operation operationVM);

        void GoBack();

        void GoForward();

        void ClearNavigationStack();
    }
}
