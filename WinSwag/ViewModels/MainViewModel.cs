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
            private set => Set(ref _specification, value);
        }

        public OperationViewModel SelectedOperation
        {
            get => _selectedOperation;
            set => Set(ref _selectedOperation, value);
        }

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
            MessengerInstance.Register<SpecificationLoaded>(this, msg =>
                Specification = new SwaggerSpecificationViewModel(msg.Specification));

            MessengerInstance.Register<AppMessage>(this, msg =>
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
