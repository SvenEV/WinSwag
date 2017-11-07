using System.ComponentModel;
using WinSwag.ViewModels;

namespace WinSwag.Services
{
    public interface IOperationManagerVM : INotifyPropertyChanged
    {
        SwaggerOperationViewModel SelectedOperation { get; set; }

        bool IsOperationSelected { get; }

        bool IsntOperationSelected { get; }
    }
}
