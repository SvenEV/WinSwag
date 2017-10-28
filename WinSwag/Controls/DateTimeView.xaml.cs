using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinSwag.Controls
{
    public sealed partial class DateTimeView : UserControl
    {
        public static readonly DependencyProperty DateTimeProperty =
            DependencyProperty.Register(nameof(DateTime), typeof(DateTimeOffset?), typeof(DateTimeView), new PropertyMetadata(null, OnDateTimeChanged));

        private static void OnDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((DateTimeView)d).OnDateTimeChanged((DateTimeOffset?)e.NewValue);

        public DateTimeOffset? DateTime
        {
            get { return (DateTimeOffset?)GetValue(DateTimeProperty); }
            set { SetValue(DateTimeProperty, value); }
        }

        public DateTimeView() => InitializeComponent();

        private void OnDateTimeChanged(DateTimeOffset? newValue)
        {
            Time.Time = newValue?.TimeOfDay ?? TimeSpan.Zero;
            Date.SelectedDates.Clear();
            if (newValue.HasValue)
                Date.SelectedDates.Add(newValue.Value);
        }

        private void OnDateChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (Date.SelectedDates.Any())
                DateTime = Date.SelectedDates[0].Date + Time.Time;
        }

        private void OnTimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            if (DateTime.HasValue)
                DateTime = DateTime.Value.Date + e.NewTime;
        }
    }
}
