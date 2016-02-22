using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace X2CodingLab.SensorTag.Sensors
{
    public class Movement : SensorBase
    {
        List<GattCharacteristic> _characteristics = new List<GattCharacteristic>();
        GattDeviceService _service;

        [FlagsAttribute]
        public enum MovementFlags
        {
            //None = 0,

            ///// <summary>
            ///// Enable Gyro X-Axis
            ///// </summary>
            //GyroX = 1,

            ///// <summary>
            ///// Enable Gyro Y-Axis
            ///// </summary>
            //GyroY = 2,

            ///// <summary>
            ///// Enable Gyro Z-Axis
            ///// </summary>
            //GyroZ = 4,

            ///// <summary>
            ///// Enable Accelerometer X-Axis
            ///// </summary>
            //AccelX = 8,
            ///// <summary>
            ///// Enable Accelerometer Y-Axis
            ///// </summary>
            ///// 
            //AccelY = 0x10,

            ///// <summary>
            ///// Enable Accelerometer Z-Axis
            ///// </summary>
            //AccelZ = 0x20,

            ///// <summary>
            ///// Enable Magnetometer 
            ///// </summary>
            //Mag = 0x40,

            ///// <summary>
            ///// The Wake-On-Motion (WOM) feature allows the MPU to operate with only the accelerometer enabled, but will give an interrupt to the CC2650 when motion is detected. 
            ///// After a shake is detected, the SensorTag will provide movement data for 10 seconds before entering the MPU re-enters low power WOM state
            ///// </summary>
            //WakeOnMotion = 0x80,

            ///// <summary>
            ///// Accelerometer range (2G)
            ///// </summary>
            //Accel2G = 0,

            ///// <summary>
            ///// Accelerometer range (4G)
            ///// </summary>
            //Accel4G = 0x100,

            ///// <summary>
            ///// Accelerometer range (8G)
            ///// </summary>
            //Accel8G = 0x200,

            ///// <summary>
            ///// Accelerometer range (16G)
            ///// </summary>

        }

        public class MovementMeasurement
        {
            /// <summary>
            /// Get/Set X accelerometer in units of 1 g (9.81 m/s^2).
            /// </summary>
            public double AccelX { get; set; }

            /// <summary>
            /// Get/Set Y accelerometer in units of 1 g (9.81 m/s^2).
            /// </summary>
            public double AccelY { get; set; }

            /// <summary>
            /// Get/Set Z accelerometer in units of 1 g (9.81 m/s^2).
            /// </summary>
            public double AccelZ { get; set; }

            /// <summary>
            /// Get/Set X twist in degrees per second.
            /// </summary>
            public double GyroX { get; set; }

            /// <summary>
            /// Get/Set Y twist in degrees per second.
            /// </summary>
            public double GyroY { get; set; }

            /// <summary>
            /// Get/Set Z twist in degrees per second.
            /// </summary>
            public double GyroZ { get; set; }

            /// <summary>
            /// Get/Set X direction in units of 1 micro tesla.
            /// </summary>
            public double MagX { get; set; }

            /// <summary>
            /// Get/Set Y direction in units of 1 micro tesla.
            /// </summary>
            public double MagY { get; set; }

            /// <summary>
            /// Get/Set Z direction in units of 1 micro tesla.
            /// </summary>
            public double MagZ { get; set; }

            public MovementMeasurement()
            {
            }

        }
        public Movement()
            : base(SensorName.Movement, SensorTagUuid.UUID_MOV_SERV, SensorTagUuid.UUID_MOV_CONF, SensorTagUuid.UUID_MOV_DATA)
        {

        }

        public override async Task EnableSensor()
        {
            // One bit for each gyro and accelerometer axis (6), magnetometer (1), wake-on-motion enable (1), accelerometer range (2). 
            // Write any bit combination top enable the desired features
            //byte[] _movConfig = BitConverter.GetBytes((ushort)(MovementFlags.WakeOnMotion | MovementFlags.Accel2G | MovementFlags.AccelX | MovementFlags.AccelY | MovementFlags.AccelZ | MovementFlags.GyroX | MovementFlags.GyroY | MovementFlags.GyroZ));
            byte[] _movConfig = new byte[] { (byte)0x7F, (byte)0x00 };
            await base.EnableSensor(_movConfig);
        }

        public static MovementMeasurement GetMovementMeasurements(byte[] sensorData)
        {
            if (sensorData.Length == 18)
            {
                MovementMeasurement measurement = new MovementMeasurement();
                short gx = BitConverter.ToInt16(sensorData,0);
                short gy = BitConverter.ToInt16(sensorData,2);
                short gz = BitConverter.ToInt16(sensorData,4);
                short ax = BitConverter.ToInt16(sensorData,6);
                short ay = BitConverter.ToInt16(sensorData,8);
                short az = BitConverter.ToInt16(sensorData,10);
                short mx = BitConverter.ToInt16(sensorData,12);
                short my = BitConverter.ToInt16(sensorData,14);
                short mz = BitConverter.ToInt16(sensorData,16);

                measurement.GyroX = ((double)gx * 500.0) / 65536.0;
                measurement.GyroY = ((double)gy * 500.0) / 65536.0;
                measurement.GyroZ = ((double)gz * 500.0) / 65536.0;

                measurement.AccelX = ((double)ax / 32768);
                measurement.AccelY = ((double)ay / 32768);
                measurement.AccelZ = ((double)az / 32768);

                // on SensorTag CC2650 the conversion to micro tesla's is done in the firmware.
                measurement.MagX = (double)mx;
                measurement.MagY = (double)my;
                measurement.MagZ = (double)mz;

                return measurement;

            }

            return null;

        }

        protected short ReadBigEndian16bit(DataReader reader)
        {
            byte lo = reader.ReadByte();
            byte hi = reader.ReadByte();
            return (short)(((short)hi << 8) + (short)lo);
        }
    }
}
