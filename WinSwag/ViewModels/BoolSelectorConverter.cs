using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WinSwag.ViewModels
{
    /// <summary>
    /// Returns one of two values depending on an input boolean value.
    /// </summary>
    class BoolSelectorConverter : DependencyObject, IValueConverter
    {
        public static readonly DependencyProperty TrueValueProperty =
            DependencyProperty.Register(nameof(TrueValue), typeof(object), typeof(BoolSelectorConverter), new PropertyMetadata(null));

        public static readonly DependencyProperty FalseValueProperty =
            DependencyProperty.Register(nameof(FalseValue), typeof(object), typeof(BoolSelectorConverter), new PropertyMetadata(null));

        public static readonly DependencyProperty FallbackValueProperty =
            DependencyProperty.Register(nameof(FallbackValue), typeof(object), typeof(BoolSelectorConverter), new PropertyMetadata(null));

        public object TrueValue
        {
            get => GetValue(TrueValueProperty);
            set => SetValue(TrueValueProperty, value);
        }

        public object FalseValue
        {
            get => GetValue(FalseValueProperty);
            set => SetValue(FalseValueProperty, value);
        }

        public object FallbackValue
        {
            get => GetValue(FallbackValueProperty);
            set => SetValue(FallbackValueProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (value is bool b)
                ? (b ? TrueValue : FalseValue)
                : FallbackValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
