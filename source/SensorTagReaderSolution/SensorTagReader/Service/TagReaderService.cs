using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using X2CodingLab.SensorTag;
using X2CodingLab.SensorTag.Sensors;

namespace SensorTagReader.Service
{
    public class SensorValues
    {
        public string HorseName { get; set; }
        public string SessionID { get; set; }
        public string SensorSystemID { get; set; }
        public string SensorFriendlyName { get; set; }
        public double Humidity { get; set; }
        public double Temperature { get; set; }
        public Movement.MovementMeasurement Movement { get; set; }
    }

    public class TagReaderService
    {
        string _sensorSystemID;
        string _sensorFriendlyName;
        HumiditySensor _humiditySensor;
        IRTemperatureSensor _tempSensor;
        Movement _movement;
        DeviceInfoService _deviceInfoService = new DeviceInfoService();
        
        public SensorValues CurrentValues { get; set; }        
        public TagReaderService()
        {
            _humiditySensor = new HumiditySensor();
            _tempSensor = new IRTemperatureSensor();
            _movement = new Movement();
            CurrentValues = new SensorValues();
        }
        public async Task<string> TagsText()
        {
            List<DeviceInformation> tags = await GattUtils.GetDevicesOfService(_tempSensor.SensorServiceUuid);
            if (tags != null)
                return "Total: " + tags.Count();
            else
                return String.Empty;
        }
        public async Task<string> GetSensorID(GattDeviceService deviceService)
        {
            Exception exc = null;
            string SensorData = "";

            try
            {
                

                using (this._deviceInfoService)
                 {
                    //await this._deviceInfoService.Initialize();
                                        
                    //foreach (GattDeviceService deviceService in _deviceInfoService.deviceServices)
                    //{
                    _deviceInfoService.deviceService = deviceService;

                    // read these values so that I can save them
                    _sensorSystemID = await _deviceInfoService.ReadSystemId();
                    string _serialNumber = await _deviceInfoService.ReadSerialNumber();
                    if (_sensorSystemID == SensorIDMappings.Left_Front) _sensorFriendlyName = "Left Front";
                    if (_sensorSystemID == SensorIDMappings.Right_Front) _sensorFriendlyName = "Right Front";
                    if (_sensorSystemID == SensorIDMappings.Left_Hind) _sensorFriendlyName = "Left Hind";
                    if (_sensorSystemID == SensorIDMappings.Right_Hind) _sensorFriendlyName = "Right Hind";

                    SensorData += "Friendly Name: " + _sensorFriendlyName + "\n";
                    SensorData += "System ID: " + _sensorSystemID + "\n";
                    SensorData += "Model Nr: " + await _deviceInfoService.ReadModelNumber() + "\n";
                    SensorData += "Serial Nr: " + _serialNumber + "\n";
                    SensorData += "Firmware Revision: " + await _deviceInfoService.ReadFirmwareRevision() + "\n";
                    SensorData += "Hardware Revision: " + await _deviceInfoService.ReadHardwareRevision() + "\n";
                    SensorData += "Sofware Revision: " + await _deviceInfoService.ReadSoftwareRevision() + "\n";
                    SensorData += "Manufacturer Name: " + await _deviceInfoService.ReadManufacturerName() + "\n";
                    SensorData += "Cert: " + await _deviceInfoService.ReadCert() + "\n";
                    SensorData += "PNP ID: " + await _deviceInfoService.ReadPnpId();
                    SensorData += "\n\n";

                    // save the friendly name and the system id
                    CurrentValues.SensorFriendlyName = _sensorFriendlyName;
                    CurrentValues.SensorSystemID = _sensorSystemID;
                    }
                    
                //}

                return SensorData;
            }
            catch (Exception ex)
            {
                exc = ex;
            }

            if (exc != null)
                SensorData += exc.Message;

            return SensorData;
        }
        public async Task<string> InitializeSensor()
        {
            await _movement.Initialize();
            await _movement.EnableSensor();
            await _humiditySensor.Initialize();
            await _humiditySensor.EnableSensor();
            await _tempSensor.Initialize();
            await _tempSensor.EnableSensor();

            _humiditySensor.SensorValueChanged += SensorValueChanged;
            _tempSensor.SensorValueChanged += SensorValueChanged;
            _movement.SensorValueChanged += SensorValueChanged;

            await _humiditySensor.EnableNotifications();
            await _tempSensor.EnableNotifications();
            await _movement.EnableNotifications();

            return ("done");
        }
        private void SensorValueChanged(object sender, X2CodingLab.SensorTag.SensorValueChangedEventArgs e)
        {
            switch (e.Origin)
            {
                case SensorName.HumiditySensor:
                    CurrentValues.Humidity = HumiditySensor.CalculateHumidityInPercent(e.RawData);
                    break;
                case SensorName.TemperatureSensor:
                    CurrentValues.Temperature = IRTemperatureSensor.CalculateAmbientTemperature(e.RawData, TemperatureScale.Celsius);
                    break;
                case SensorName.Movement:
                    CurrentValues.Movement = Movement.GetMovementMeasurements(e.RawData);
                    break;
            }
        }
    }
}
