using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace WinSwag.Controls
{
    /// <summary>
    /// A simplified <see cref="HyperlinkButton"/> that automatically launches the
    /// URI on click and shows the URI in a tooltip.
    /// </summary>
    public class LinkButton : HyperlinkButton
    {
        public LinkButton()
        {
            Padding = new Thickness(0, 2, 0, 2);
            Click += OnClicked;

            SetBinding(ToolTipService.ToolTipProperty, new Binding
            {
                Path = new PropertyPath(nameof(NavigateUri)),
                Source = this
            });

            MenuFlyoutItem openButton, copyLinkButton;

            ContextFlyout = new MenuFlyout
            {
                Items =
                {
                    (openButton = new MenuFlyoutItem
                    {
                        Text = "Open",
                        Icon = new SymbolIcon(Symbol.Go)
                    }),
                    (copyLinkButton = new MenuFlyoutItem
                    {
                        Text = "Copy link",
                        Icon = new SymbolIcon(Symbol.Copy)
                    })
                }
            };

            openButton.Click += OnClicked;
            copyLinkButton.Click += OnCopyLinkClicked;
        }

        private async void OnClicked(object sender, RoutedEventArgs e)
        {
            if (NavigateUri != default(Uri))
                await Launcher.LaunchUriAsync(NavigateUri);
        }

        private void OnCopyLinkClicked(object sender, RoutedEventArgs e)
        {
            if (NavigateUri != default(Uri))
            {
                var package = new DataPackage();
                package.SetText(NavigateUri.ToString());
                Clipboard.SetContent(package);
                Clipboard.Flush();
            }
        }
    }
}
