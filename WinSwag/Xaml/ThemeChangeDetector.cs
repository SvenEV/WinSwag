using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinSwag.Xaml
{
    public class ThemeChangeDetector : Grid
    {
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register(nameof(Theme), typeof(string), typeof(ThemeChangeDetector), new PropertyMetadata(null, OnThemeChanged));

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((ThemeChangeDetector)d).ThemeChanged?.Invoke((string)e.NewValue);

        public string Theme
        {
            get => (string)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public event Action<string> ThemeChanged;
    }
}
