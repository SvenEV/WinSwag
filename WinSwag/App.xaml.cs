using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinSwag.Core;
using WinSwag.Services;
using WinSwag.ViewModels;

namespace WinSwag
{
    sealed partial class App : Application
    {
        private IServiceProvider _services;

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;

            var services = new ServiceCollection();
            ConfigureServices(services);
            _services = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<ApplicationInfo>()
                .AddSingleton<SwaggerSessionManager>()
                .AddScoped<IMessenger, Messenger>()
                .AddScoped<ISessionManagerVM, SessionManagerVM>()
                .AddScoped<IOperationManagerVM, OperationManagerVM>()
                .AddScoped<IViewStateManagerVM, ViewStateManagerVM>();

            // Waiting for a fix for https://developercommunity.visualstudio.com/content/problem/130643/cant-build-release-when-i-use-microsoftservicessto.html
            //var engagementManager = StoreServicesEngagementManager.GetDefault();
            //await engagementManager.RegisterNotificationChannelAsync();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // TODO: Properly handle suspend/resume and prelaunch
            if (rootFrame == null)
            {
                ApplicationInstance.InitializeForCurrentView(_services);
            }

            // TODO: Uncomment to enable multi-instance support
            //else
            //{
            //    await ApplicationInstance.LaunchNewAsync(_services);
            //}
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            
            // Save all open favored sessions
            foreach (var instance in ApplicationInstance.All)
            {
                var sessionManager = instance.Services.GetService<ISessionManagerVM>();
                sessionManager.UnloadCurrentSessionAsync();
            }

            deferral.Complete();
        }
    }
}
