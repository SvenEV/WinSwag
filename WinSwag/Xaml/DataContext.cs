using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinSwag.Xaml
{
    /// <summary>
    /// Provides an attached property "Model" which, when assigned to an element, sets the DataContext
    /// of that element to a view model instance for that model object.
    /// </summary>
    public class DataContext : Grid
    {
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.RegisterAttached("Model", typeof(object), typeof(DataContext), new PropertyMetadata(null, OnModelChanged));

        public static object GetModel(DependencyObject obj) => obj.GetValue(ModelProperty);

        public static void SetModel(DependencyObject obj, object value) => obj.SetValue(ModelProperty, value);

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)d;
            element.DataContext = ViewModelRegistry.ViewModelFor(e.NewValue);
        }
    }

    /// <summary>
    /// An invisible element providing a viewmodel for a specified model object.
    /// </summary>
    public class BindingContext : FrameworkElement
    {
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register(nameof(Model), typeof(object), typeof(BindingContext), new PropertyMetadata(null, OnModelChanged));

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(BindingContext), new PropertyMetadata(null));

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(ViewModelProperty, ViewModelRegistry.ViewModelFor(e.NewValue));
        }

        public object Model
        {
            get => GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        public object ViewModel => GetValue(ViewModelProperty);

        public BindingContext()
        {
            Visibility = Visibility.Collapsed;
            IsHitTestVisible = false;
        }
    }
}
