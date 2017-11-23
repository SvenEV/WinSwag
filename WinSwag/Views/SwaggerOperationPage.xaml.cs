using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinSwag.Core;
using WinSwag.ViewModels;
using WinSwag.Xaml;

namespace WinSwag.Views
{
    public sealed partial class SwaggerOperationPage : Page
    {
        public static readonly DependencyProperty OperationProperty =
            DependencyProperty.Register(nameof(Operation), typeof(Operation), typeof(SwaggerOperationPage), new PropertyMetadata(null));

        public static readonly DependencyProperty VMProperty =
            DependencyProperty.Register(nameof(VM), typeof(OperationViewModel), typeof(SwaggerOperationPage), new PropertyMetadata(null));

        public Operation Operation
        {
            get { return (Operation)GetValue(OperationProperty); }
            set { SetValue(OperationProperty, value); }
        }

        public OperationViewModel VM
        {
            get { return (OperationViewModel)GetValue(VMProperty); }
            set { SetValue(VMProperty, value); }
        }

        public SwaggerOperationPage()
        {
            ApplicationInstance.Current.Services.Populate(this);
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Operation = e.Parameter as Operation ?? throw new ArgumentNullException();
            VM = ViewModelRegistry.ViewModelFor<OperationViewModel>(Operation);
        }

        private void OnSendRequestButtonClicked(object sender, RoutedEventArgs e)
        {
            VM.BeginSendRequest();
        }
    }
}
