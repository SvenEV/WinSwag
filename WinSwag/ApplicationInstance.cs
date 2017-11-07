using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinSwag
{
    public class ApplicationInstance
    {
        private static readonly Dictionary<int, ApplicationInstance> _instances = new Dictionary<int, ApplicationInstance>();
        private readonly IServiceScope _services;

        public static ApplicationInstance Current => _instances[ApplicationView.GetForCurrentView().Id];

        public static IReadOnlyCollection<ApplicationInstance> All => _instances.Values;

        public IServiceProvider Services => _services.ServiceProvider;

        public CoreApplicationView CoreView { get; }

        public ApplicationView View { get; }

        public ApplicationInstance(IServiceProvider serviceProvider)
        {
            CoreView = CoreApplication.GetCurrentView();
            View = ApplicationView.GetForCurrentView();
            _services = serviceProvider.CreateScope();
        }

        public static void InitializeForCurrentView(IServiceProvider serviceProvider)
        {
            var viewId = ApplicationView.GetForCurrentView().Id;

            if (_instances.ContainsKey(viewId))
                throw new InvalidOperationException();

            var instance = new ApplicationInstance(serviceProvider);
            _instances.Add(viewId, instance);

            // Initialize window contents
            var frame = new Frame();
            frame.Navigate(typeof(MainPage));
            Window.Current.Content = frame;
            Window.Current.Activate();
        }

        public static async Task LaunchNewAsync(IServiceProvider serviceProvider)
        {
            var view = CoreApplication.CreateNewView();
            var windowId = 0;

            await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                windowId = ApplicationView.GetApplicationViewIdForWindow(CoreWindow.GetForCurrentThread());
                InitializeForCurrentView(serviceProvider);
            });

            //ApplicationViewSwitcher.DisableSystemViewActivationPolicy();
            var b = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(windowId);
        }
    }
}
