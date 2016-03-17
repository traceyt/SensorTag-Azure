using SensorTagReader.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using X2CodingLab.SensorTag.Sensors;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SensorTagReader.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {

        DispatcherTimer eventHubWriterTimer;
        //TagReaderService tagreader;
        List<TagReaderService> tagReaders;
        DeviceInfoService deviceInfoService;
        EventHubService eventHubService;
        int numberOfCallsDoneToEventHub;
        int numberOfFailedCallsToEventHub;
        double currentSimulatedTemperature = 21.0F;
        Random simulatorRandomizer = new Random();

        // set the horse name and session id
        string _sessionID = Guid.NewGuid().ToString();

        Windows.Storage.ApplicationDataContainer localSettings;

        App app;


        public HomePage()
        {
            this.InitializeComponent();

            localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            StatusField.Text = "Please ensure the sensor is connected";

            app = App.Current as SensorTagReader.App;
            HorseNameField.Text = app.HorseName;

            tagReaders = new List<TagReaderService>();
            deviceInfoService = new DeviceInfoService();

            eventHubWriterTimer = new DispatcherTimer();
            eventHubWriterTimer.Interval = new TimeSpan(0, 0, 1);
            eventHubWriterTimer.Tick += OnEventHubWriterTimerTick;

        }


        private async void OnEventHubWriterTimerTick(object sender, object e)
        {
            if ((string)StartCommand.Tag == "STARTED")
            {
                foreach (TagReaderService tagreader in tagReaders)
                {
                    if (tagreader == null || tagreader.CurrentValues == null)
                        return;

                    //txtTemperature.Text = $"{tagreader.CurrentValues.Temperature:N2} C";
                    //txtHumidity.Text = $"{tagreader.CurrentValues.Humidity:N2} %";

                    try
                    {
                        await eventHubService.SendMessage(new Messages.EventHubSensorMessage()
                        {
                            HorseName = HorseNameField.Text,
                            SessionID = SesssionIDField.Text,
                            SensorName = "SensorData", //SensorNameField.Text,
                            SensorFriendlyName = tagreader.CurrentValues.SensorFriendlyName,
                            SensorSystemID = tagreader.CurrentValues.SensorSystemID,
                            TimeWhenRecorded = DateTime.Now,
                            Temperature = tagreader.CurrentValues.Temperature,
                            Humidity = tagreader.CurrentValues.Humidity,
                            Movement = tagreader.CurrentValues.Movement
                        });
                        numberOfCallsDoneToEventHub++;
                    }
                    catch { numberOfFailedCallsToEventHub++; }
                }

            }
            else
            {
                setNextSimulatedValue();

                // txtTemperature.Text = $"{currentSimulatedTemperature:N2} C";

                try
                {
                    await eventHubService.SendMessage(new Messages.EventHubSensorMessage()
                    {
                        SensorName = "SensorData", // SensorNameField.Text,
                        TimeWhenRecorded = DateTime.Now,
                        Temperature = currentSimulatedTemperature,
                        Humidity = 50
                    });
                    numberOfCallsDoneToEventHub++;
                }
                catch { numberOfFailedCallsToEventHub++; }

            }

            EventHubInformation.Text = $"Calls: {numberOfCallsDoneToEventHub}, Failed Calls: {numberOfFailedCallsToEventHub}";

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private async void StartCommand_Click(object sender, RoutedEventArgs e)
        {
            if ((string)StartCommand.Tag == "STOPPED")
            {
                try
                {
                    await startTracking();
                }
                catch (Exception ex)
                {
                    txtError.Text = ex.Message;
                }
            }
            else
            {
                stopTracking();
            }
        }

        private void stopTracking()
        {
            StartCommand.Content = "Start";
            StartCommand.Tag = "STOPPED";
            eventHubWriterTimer.Stop();
        }

        private async Task startTracking()
        {
            // get the list of devices to track
            await deviceInfoService.Initialize();

            SensorInformation.Text = "";
            foreach (GattDeviceService deviceService in deviceInfoService.deviceServices)
            {
                TagReaderService tagReader = new TagReaderService();
                await tagReader.InitializeSensor();
                SensorInformation.Text += await tagReader.GetSensorID(deviceService);
                if (tagReader != null)
                    this.tagReaders.Add(tagReader);
            }


            // eventHubService = new EventHubService(ServiceBusNamespace,
            //EventHubNameField.Text, SharedAccessPolicyNameField.Text, SharedAccessPolicyKeyField.Text);

            StatusField.Text = "The sensor is connected";
            txtError.Text = "";
            eventHubWriterTimer.Start();
            StartCommand.Content = "Stop";
            StartCommand.Tag = "STARTED";


            numberOfFailedCallsToEventHub = numberOfCallsDoneToEventHub = 0;
            EventHubInformation.Text = $"Calls: {numberOfCallsDoneToEventHub}, Failed Calls: {numberOfFailedCallsToEventHub}";
        }

        //private async Task startSimulation()
        //{
        //    eventHubService = new EventHubService(ServiceBusNamespaceField.Text,
        //        EventHubNameField.Text, SharedAccessPolicyNameField.Text, SharedAccessPolicyKeyField.Text);

        //    //SimulateCommand.Content = "Stop";
        //    SimulateCommand.Tag = "STARTED";
        //    txtError.Text = "";
        //    eventHubWriterTimer.Start();
        //    numberOfFailedCallsToEventHub = numberOfCallsDoneToEventHub = 0;
        //    EventHubInformation.Text = $"Calls: {numberOfCallsDoneToEventHub}, Failed Calls: {numberOfFailedCallsToEventHub}";
        //}

        //private void stopSimulation()
        //{
        //    SimulateCommand.Content = "Start";
        //    SimulateCommand.Tag = "STOPPED";
        //    eventHubWriterTimer.Stop();
        //}

        private void OnSettingsChanged(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null)
            {
                if (textBox.Name == "HorseNameField")
                    app.HorseName = textBox.Text;
                stopTracking();
            }
        }

        //private async void SimulateCommand_Click(object sender, RoutedEventArgs e)
        //{
        //    if ((string)SimulateCommand.Tag == "STOPPED")
        //    {
        //        try
        //        {
        //            await startSimulation();
        //        }
        //        catch (Exception ex)
        //        {
        //            txtError.Text = ex.Message;
        //        }
        //    }
        //    else
        //    {
        //        stopSimulation();
        //    }
        //}

        private void setNextSimulatedValue()
        {
            currentSimulatedTemperature += simulatorRandomizer.Next(-1, 2) * 0.5;
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private async void EnableRemote_Click(object sender, RoutedEventArgs e)
        {
            await EnableRemote();
        }

        private async void DisableRemote_Click(object sender, RoutedEventArgs e)
        {
            await DisableRemote();
        }

        private async Task EnableRemote()
        {
            // ring buzzer of the first device
            await tagReaders[0].CurrentValues.IOService.EnableRemote();
        }

        private async Task DisableRemote()
        {
            // ring buzzer of the first device
            await tagReaders[0].CurrentValues.IOService.DisableRemote();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnMenuButtonClicked(object sender, RoutedEventArgs e)
        {

        }

        private void OnHomeButtonChecked(object sender, RoutedEventArgs e)
        {

        }

        private void OnSearchButtonChecked(object sender, RoutedEventArgs e)
        {

        }

        private void OnSettingsButtonChecked(object sender, RoutedEventArgs e)
        {

        }

        private void OnAboutButtonChecked(object sender, RoutedEventArgs e)
        {

        }

        private void SimulateCommand_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}