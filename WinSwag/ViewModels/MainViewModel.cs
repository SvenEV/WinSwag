using GalaSoft.MvvmLight;

namespace WinSwag.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private SwaggerSpecificationViewModel _specification;
        private OperationViewModel _selectedOperation;
        private bool _isBusy;

        public SwaggerSpecificationViewModel Specification
        {
            get => _specification;
            private set
            {
                if (Set(ref _specification, value) && value == null)
                    SelectedOperation = null;
            }
        }

        public OperationViewModel SelectedOperation
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

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (Set(ref _isBusy, value))
                    RaisePropertyChanged(nameof(IsIdle));
            }
        }

        public bool IsIdle => !_isBusy;

        public MainViewModel()
        {
            App.CurrentMessenger.Register<SpecificationLoaded>(this, msg =>
                Specification = new SwaggerSpecificationViewModel(msg.Specification));

            App.CurrentMessenger.Register<AppMessage>(this, msg =>
            {
                switch (msg)
                {
                    case AppMessage.ClearCurrentSpecification: Specification = null; break;
                    case AppMessage.BeginLoad: IsBusy = true; break;
                    case AppMessage.EndLoad: IsBusy = false; break;
                }
            });
        }
    }
}
