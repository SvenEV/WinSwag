﻿using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WinSwag
{
    sealed partial class App : Application
    {
        private static readonly Dictionary<int, Messenger> _messengers = new Dictionary<int, Messenger>();

        public static Messenger CurrentMessenger
        {
            get
            {
                var viewId = ApplicationView.GetApplicationViewIdForWindow(Window.Current.CoreWindow);
                return _messengers.TryGetValue(viewId, out var m) ? m : _messengers[viewId] = new Messenger();
            }
        }

        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

                // TODO: Think about that!
                if (e.PrelaunchActivated == false)
                {
                    if (rootFrame.Content == null)
                    {
                        // When the navigation stack isn't restored navigate to the first page,
                        // configuring the new page by passing required information as a navigation
                        // parameter
                        rootFrame.Navigate(typeof(MainPage), e.Arguments);
                    }
                    // Ensure the current window is active
                    Window.Current.Activate();
                }
            }
            else
            {
                var view = CoreApplication.CreateNewView();
                var windowId = 0;

                await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    windowId = ApplicationView.GetApplicationViewIdForWindow(CoreWindow.GetForCurrentThread());
                    var frame = new Frame();
                    frame.Navigate(typeof(MainPage), null);
                    Window.Current.Content = frame;
                    Window.Current.Activate();
                });

                //ApplicationViewSwitcher.DisableSystemViewActivationPolicy();
                var b = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(windowId);
            }


        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
