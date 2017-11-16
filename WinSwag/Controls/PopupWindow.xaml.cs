using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace WinSwag.Controls
{
    [ContentProperty(Name = nameof(InnerContent))]
    public sealed partial class PopupWindow : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(PopupWindow), new PropertyMetadata(""));

        public static readonly DependencyProperty InnerContentProperty =
            DependencyProperty.Register(nameof(InnerContent), typeof(object), typeof(PopupWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(PopupWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(PopupWindow), new PropertyMetadata(false));

        public event TypedEventHandler<PopupWindow, object> Opened;

        public event TypedEventHandler<PopupWindow, object> Closed;

        /// <summary>
        /// An invisible element at the top of the window that can be assigned as the current window's
        /// title bar while the popup is open and the view extends into the title bar area.
        /// </summary>
        public UIElement TitleBar => TitleBarElement;

        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public object InnerContent
        {
            get => GetValue(InnerContentProperty);
            set => SetValue(InnerContentProperty, value);
        }

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public PopupWindow()
        {
            InitializeComponent();
            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // on phone, the root is offset by about 22 pixels because of the tray
            // thus, we have to subtract the root offset from the parent offset

            var parent = Parent as UIElement;
            var parentTransform = TransformToVisual(parent);
            var parentOffset = parentTransform.TransformPoint(new Point(0, 0));

            popup.HorizontalOffset = parentOffset.X;
            popup.VerticalOffset = parentOffset.Y;

            if (popup.Child is FrameworkElement child)
            {
                child.Width = ActualWidth;
                child.Height = ActualHeight;
            }
        }

        public void Show() => IsOpen = true;

        public void Hide() => IsOpen = false;

        public void ShowWithData(object data)
        {
            InnerContent = data;
            Show();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = false;
        }

        private void OnOpened(object sender, object e) => Opened?.Invoke(this, null);

        private void OnClosed(object sender, object e) => Closed?.Invoke(this, null);
    }
}
