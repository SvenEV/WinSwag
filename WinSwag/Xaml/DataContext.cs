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
            var dataContext = (FrameworkElement)d;
            dataContext.DataContext = ViewModelRegistry.ViewModelFor(e.NewValue);
        }
    }
}
