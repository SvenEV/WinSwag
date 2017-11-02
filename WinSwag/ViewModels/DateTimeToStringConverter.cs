using System;
using Windows.UI.Xaml.Data;

namespace WinSwag.ViewModels
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset dateTimeOffset)
                return dateTimeOffset.ToString("s");
            if (value is DateTime dateTime)
                return dateTime.ToString("s");
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (DateTimeOffset.TryParse(value?.ToString() ?? "", out var result))
                return result;
            return default(DateTimeOffset?);
        }
    }
}
