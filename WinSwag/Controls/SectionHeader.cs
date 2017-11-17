using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinSwag.Controls
{
    public class SectionHeader : Control
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SectionHeader), new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public SectionHeader() => DefaultStyleKey = typeof(SectionHeader);
    }
}
