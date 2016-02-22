using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using X2CodingLab.SensorTag.Exceptions;
using X2CodingLab.Utils;

namespace X2CodingLab.SensorTag.Sensors
{
    public class IOService : SensorBase
    {
        GattCharacteristic dataCharacteristic;
        GattCharacteristic configCharacteristic;

        [FlagsAttribute]
        public enum ActivatingFlags
        {
            Red = 0,

            /// <summary>
            /// Enable Gyro X-Axis
            /// </summary>
            GyroX = 1,

            /// <summary>
            /// Enable Gyro Y-Axis
            /// </summary>
            GyroY = 2,

            /// <summary>
            /// Enable Gyro Z-Axis
            /// </summary>
            GyroZ = 4,
        }

        public IOService()
            : base(SensorName.IOService, SensorTagUuid.UUID_IO_SERV, SensorTagUuid.UUID_IO_CONF, SensorTagUuid.UUID_IO_DATA)
        {

        }

        /// <summary>
        /// Enables data notifications from the sensor by setting the configurationDescriptorvalue to Notify.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DeviceUnreachableException">Thrown if it wasn't possible to communicate with the device.</exception>
        /// <exception cref="DeviceNotInitializedException">Thrown if the object has not been successfully initialized using the initialize() method.</exception>
        public async Task EnableService()
        {
            Validator.Requires<DeviceNotInitializedException>(deviceService != null);

            configCharacteristic = deviceService.GetCharacteristics(new Guid(this.SensorConfigUuid))[0];

        }

        public async Task EnableRemote()
        {
            GattCommunicationStatus status;
            byte[] sensorData;

            Validator.Requires<DeviceNotInitializedException>(deviceService != null);

            configCharacteristic = deviceService.GetCharacteristics(new Guid(this.SensorConfigUuid))[0];
            dataCharacteristic = deviceService.GetCharacteristics(new Guid(this.SensorDataUuid))[0];


            sensorData = new byte[] { 1 };
            status = await configCharacteristic.WriteValueAsync(sensorData.AsBuffer());

            sensorData = new byte[] { 1 };
            status = await dataCharacteristic.WriteValueAsync(sensorData.AsBuffer());


        }


        public async Task DisableRemote()
        {
            Validator.Requires<DeviceNotInitializedException>(deviceService != null);

            configCharacteristic = deviceService.GetCharacteristics(new Guid(this.SensorConfigUuid))[0];
            dataCharacteristic = deviceService.GetCharacteristics(new Guid(this.SensorDataUuid))[0];

            byte[] sensorData = new byte[] { 0 };

            GattCommunicationStatus status = await configCharacteristic.WriteValueAsync(sensorData.AsBuffer());

        }
    }
}
