using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WinSwag.Xaml
{
    public static class X
    {
        public static Visibility NotEmpty(object o)
        {
            return (o is string s ? string.IsNullOrWhiteSpace(s) : o == null)
                ? Visibility.Collapsed : Visibility.Visible;
        }

        public static Visibility Eq(object o, string s) =>
            Equals(o, s) || Equals(o?.ToString(), s) ? Visibility.Visible : Visibility.Collapsed;

        public static Visibility Not(bool b) => b ? Visibility.Collapsed : Visibility.Visible;
    }

    public class DelegateConverter<TIn, TOut> : IValueConverter
    {
        private readonly Func<TIn, TOut> _convert;
        private readonly Func<TOut, TIn> _convertBack;

        public DelegateConverter(Func<TIn, TOut> convert, Func<TOut, TIn> convertBack = null)
        {
            _convert = convert ?? throw new ArgumentNullException(nameof(convert));
            _convertBack = convertBack ?? throw new ArgumentNullException(nameof(convertBack));
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is TIn v)
                return _convert(v);

            throw new ArgumentException($"Converter expected value of type '{typeof(TIn).Name}', but got value of type '{value?.GetType().Name}'");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (_convertBack == null)
                throw new NotSupportedException("Converter does not support backwards conversion");

            if (value is TOut v)
                return _convertBack(v);

            throw new ArgumentException($"Converter expected value of type '{typeof(TOut).Name}', but got value of type '{value?.GetType().Name}'");
        }
    }
}
