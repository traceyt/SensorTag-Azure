using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SensorTagReader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {

        Windows.Storage.ApplicationDataContainer localSettings;
        App app;


        public SettingsPage()
        {
            this.InitializeComponent();

            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            app = App.Current as SensorTagReader.App ;

            ServiceBusNamespaceField.Text = app.ServiceBusNamespace;
            EventHubNameField.Text = app.EventHubName;
            SharedAccessPolicyNameField.Text = app.SharedAccessPolicyName;
            SharedAccessPolicyKeyField.Text = app.SharedAccessPolicyKey;
            SensorNameField.Text = app.SensorName;

            // getVersionNumberOfApp();

        }

        private void getVersionNumberOfApp()
        {
            var thisPackage = Windows.ApplicationModel.Package.Current;
            var version = thisPackage.Id.Version;

            VersionField.Text = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }


        private void OnSettingsChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                localSettings.Values[textBox.Name] = textBox.Text;

                if (textBox.Name == "ServiceBusNamespaceField")
                    app.ServiceBusNamespace = textBox.Text;

                if (textBox.Name == "EventHubNameField")
                    app.EventHubName = textBox.Text;

                if (textBox.Name == "SharedAccessPolicyNameField")
                    app.SharedAccessPolicyName = textBox.Text;

                if (textBox.Name == "SharedAccessPolicyKeyField")
                    app.SharedAccessPolicyKey = textBox.Text;

                if (textBox.Name == "SensorNameField")
                    app.SensorName = textBox.Text;

                //stopTracking();
            }
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void ServiceBusNamespaceField_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }
}
