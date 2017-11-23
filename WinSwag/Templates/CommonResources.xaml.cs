using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using WinSwag.Core;
using WinSwag.ViewModels.ForModels;

namespace WinSwag.Templates
{
    public sealed partial class CommonResources : ResourceDictionary
    {
        public CommonResources() => InitializeComponent();

        private void OnContentTypeComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var argument = (IArgument)((FrameworkElement)sender).DataContext;
            argument.ContentType = (string)e.AddedItems.FirstOrDefault();
        }

        private void OnArgumentRootLoaded(object sender, RoutedEventArgs e)
        {
            var root = (UIElement)sender;
            var visual = ElementCompositionPreview.GetElementVisual(root);
            var anim = visual.Compositor.CreateScalarKeyFrameAnimation();
            anim.Target = nameof(UIElement.Opacity);
            anim.InsertExpressionKeyFrame(1, "this.FinalValue");
            anim.Duration = TimeSpan.FromSeconds(.5);
            var anims = visual.Compositor.CreateImplicitAnimationCollection();
            anims[nameof(UIElement.Opacity)] = anim;
            visual.ImplicitAnimations = anims;
        }

        private void OnArgumentRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var checkBox = (FrameworkElement)sender;
            var argumentVM = (ArgumentViewModelBase)checkBox.DataContext;

            if (argumentVM.IsGlobalArgument)
            {
                var flyout = checkBox.Resources["ContextFlyout"] as FlyoutBase;
                flyout.ShowAt(checkBox);
            }
        }
    }
}
