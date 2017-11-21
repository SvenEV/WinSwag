using Windows.UI.Xaml;

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
}
