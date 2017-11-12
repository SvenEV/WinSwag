using GalaSoft.MvvmLight;
using WinSwag.Services;

namespace WinSwag.ViewModels
{
    public class OperationManagerVM : ViewModelBase, IOperationManagerVM
    {
        private readonly IViewStateManagerVM _viewStateManager;
        private SwaggerOperationViewModel _selectedOperation;

        public SwaggerOperationViewModel SelectedOperation
        {
            get => _selectedOperation;
            set
            {
                if (Set(ref _selectedOperation, value))
                {
                    RaisePropertyChanged(nameof(IsOperationSelected));
                    RaisePropertyChanged(nameof(IsntOperationSelected));
                }
            }
        }

        public bool IsOperationSelected => SelectedOperation != null;

        public bool IsntOperationSelected => SelectedOperation == null;

        public OperationManagerVM(IViewStateManagerVM viewStateManager)
        {
            _viewStateManager = viewStateManager;
        }

        public void ClearSelectedOperation() => SelectedOperation = null;
    }
}
