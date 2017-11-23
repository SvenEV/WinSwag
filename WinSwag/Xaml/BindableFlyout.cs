using System.Collections;
using System.Linq;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace WinSwag.Xaml
{
    public class Box<T>
    {
        public T Action { get; }
        public Box(T action) => Action = action;
    }

    public delegate void FlyoutItemClickEventHandler(object sender, object clickedItem);

    public class BindableFlyout : DependencyObject
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached("ItemsSource", typeof(IEnumerable),
                typeof(BindableFlyout), new PropertyMetadata(null, ItemsSourceChanged));

        public static readonly DependencyProperty ItemClickProperty =
            DependencyProperty.RegisterAttached("ItemClick", typeof(Box<FlyoutItemClickEventHandler>),
                typeof(BindableFlyout), new PropertyMetadata(null));

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.RegisterAttached("DisplayMemberPath", typeof(string),
                typeof(BindableFlyout), new PropertyMetadata(null));

        public static IEnumerable GetItemsSource(DependencyObject obj) =>
            obj.GetValue(ItemsSourceProperty) as IEnumerable;

        public static void SetItemsSource(DependencyObject obj, IEnumerable value) =>
            obj.SetValue(ItemsSourceProperty, value);

        public static Box<FlyoutItemClickEventHandler> GetItemClick(DependencyObject obj) =>
            (Box<FlyoutItemClickEventHandler>)obj.GetValue(ItemClickProperty);

        public static void SetItemClick(DependencyObject obj, Box<FlyoutItemClickEventHandler> value) =>
            obj.SetValue(ItemClickProperty, value);

        public static string GetDisplayMemberPath(DependencyObject obj) =>
            (string)obj.GetValue(DisplayMemberPathProperty);

        public static void SetDisplayMemberPath(DependencyObject obj, string value) =>
            obj.SetValue(DisplayMemberPathProperty, value);

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            Setup(d as MenuFlyoutSubItem);


        private static void Setup(MenuFlyoutSubItem flyout)
        {
            if (DesignMode.DesignModeEnabled)
                return;

            var items = GetItemsSource(flyout) ?? Enumerable.Empty<object>();
            var displayMemberPath = GetDisplayMemberPath(flyout) ?? "";

            // Remove old items
            foreach (var oldItem in flyout.Items.OfType<MenuFlyoutItem>())
                oldItem.Click -= OnItemClick;

            flyout.Items.Clear();

            // Add new items
            foreach (var item in items)
            {
                var button = new MenuFlyoutItem { DataContext = item };
                button.Click += OnItemClick;
                button.SetBinding(MenuFlyoutItem.TextProperty, new Binding
                {
                    Source = item,
                    Path = new PropertyPath(displayMemberPath)
                });
                flyout.Items.Add(button);
            }

            void OnItemClick(object sender, RoutedEventArgs e)
            {
                var senderButton = (MenuFlyoutItem)sender;
                var itemClickHandler = GetItemClick(flyout) as Box<FlyoutItemClickEventHandler>;
                itemClickHandler?.Action?.Invoke(sender, senderButton.DataContext);
            }
        }
    }
}
