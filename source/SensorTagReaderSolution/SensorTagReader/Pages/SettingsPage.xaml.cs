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


        public SettingsPage()
        {
            this.InitializeComponent();

            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            Application app = Application.Current;

            int a = 1;

            //ServiceBusNamespaceField.Text = app.ServiceBusNamespace;

            //if (ServiceBusNamespaceField.Text == string.Empty)
            //{
            //    ServiceBusNamespaceField.Text = Convert.ToString(localSettings.Values["ServiceBusNamespaceField"]);
            //    ServiceBusNamespace = ServiceBusNamespaceField.Text;
            //}
            //if (EventHubNameField.Text == string.Empty)
            //{
            //    EventHubNameField.Text = Convert.ToString(localSettings.Values["EventHubNameField"]);
            //    EventHubName = EventHubNameField.Text;
            //}
            //if (SharedAccessPolicyNameField.Text == string.Empty)
            //{
            //    SharedAccessPolicyNameField.Text = Convert.ToString(localSettings.Values["SharedAccessPolicyNameField"]);
            //    SharedAccessPolicyName = SharedAccessPolicyNameField.Text;
            //}
            //if (SharedAccessPolicyKeyField.Text == string.Empty)
            //{
            //    SharedAccessPolicyKeyField.Text = Convert.ToString(localSettings.Values["SharedAccessPolicyKeyField"]);
            //    ServSharedAccessPolicyKey = SharedAccessPolicyKeyField.Text;
            //}
            // if("SensorData" == string.Empty)             SensorNameField.Text = Convert.ToString(localSettings.Values["SensorNameField"]);

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

                //if (textBox.Name == "ServiceBusNamespaceField")
                //    //ServiceBusNamespace = textBox.Text;

                //if (textBox.Name == "EventHubNameField")
                //    //EventHubName = textBox.Text;

                //if (textBox.Name == "SharedAccessPolicyNameField")
                //    //SharedAccessPolicyName = textBox.Text;

                //if (textBox.Name == "ServSharedAccessPolicyKeyField")
                //    //ServSharedAccessPolicyKey = textBox.Text;

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
