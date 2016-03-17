﻿using SensorTagReader.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SensorTagReader
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private Frame _rootFrame;

        Windows.Storage.ApplicationDataContainer localSettings;

        public string ServiceBusNamespace { get; set; }
        public string EventHubName { get; set; }
        public string SharedAccessPolicyName { get; set; }
        public string SharedAccessPolicyKey { get; set; }
        public string HorseName { get; set; }
        public string SessionID { get; set; }
        public string SensorName { get; set; }


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            //Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
            //    Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
            //    Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (Window.Current.Content == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                _rootFrame = new Frame();
                _rootFrame.NavigationFailed += OnNavigationFailed;
                _rootFrame.Navigated += OnNavigated;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = new MainPage(_rootFrame);

                // Register a handler for BackRequested events and set the
                // visibility of the Back button
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    _rootFrame.CanGoBack ?
                    AppViewBackButtonVisibility.Visible :
                    AppViewBackButtonVisibility.Collapsed;
            }

            if (_rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                _rootFrame.Navigate(typeof(HomePage), e.Arguments);
            }

            // Ensure the current window is active
            Window.Current.Activate();

            // ***********************************************
            // load values from local settings
            // ***********************************************
            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (HorseName == string.Empty) HorseName = Convert.ToString(localSettings.Values["HorseNameField"]);
            if (SessionID == string.Empty) SessionID = Guid.NewGuid().ToString(); //Convert.ToString(localSettings.Values["SessionIDField"]);

            if (ServiceBusNamespace == null) ServiceBusNamespace = Convert.ToString(localSettings.Values["ServiceBusNamespaceField"]);
            if (EventHubName == null) EventHubName = Convert.ToString(localSettings.Values["EventHubNameField"]);
            if (SharedAccessPolicyName == null) SharedAccessPolicyName = Convert.ToString(localSettings.Values["SharedAccessPolicyNameField"]);
            if (SharedAccessPolicyKey == null) SharedAccessPolicyKey = Convert.ToString(localSettings.Values["SharedAccessPolicyKeyField"]);
            if (SensorName == null) SensorName = Convert.ToString(localSettings.Values["SensorNameField"]);
            if (HorseName == null) SensorName = Convert.ToString(localSettings.Values["HorseNameField"]);

            getVersionNumberOfApp();

        }

        private void getVersionNumberOfApp()
        {
            var thisPackage = Windows.ApplicationModel.Package.Current;
            var version = thisPackage.Id.Version;

            //VersionField.Text = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
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

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            // Each time a navigation event occurs, update the Back button's visibility
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible :
                AppViewBackButtonVisibility.Collapsed;
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (_rootFrame != null && _rootFrame.CanGoBack)
            {
                e.Handled = true;
                _rootFrame.GoBack();
            }
        }
    }
}
