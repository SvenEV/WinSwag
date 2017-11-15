using System.ComponentModel;
using WinSwag.ViewModels;

namespace WinSwag.Services
{
    public interface IOperationManagerVM : INotifyPropertyChanged
    {
        SwaggerOperationViewModel SelectedOperation { get; }

        bool CanGoBack { get; }

        bool CanGoForward { get; }

        bool IsOperationSelected { get; }

        bool IsntOperationSelected { get; }

        void NavigateToApiInfo();

        void NavigateToOperation(SwaggerOperationViewModel operationVM);

        void GoBack();

        void GoForward();

        void ClearNavigationStack();
    }
}
