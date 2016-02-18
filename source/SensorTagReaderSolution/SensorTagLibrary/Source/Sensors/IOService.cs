using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using X2CodingLab.Utils;

namespace X2CodingLab.SensorTag.Sensors
{
    class IOService : SensorBase
    {
        public IOService()
                : base(SensorName.IOService, SensorTagUuid.UUID_IO_SERV, null, SensorTagUuid.UUID_IO_DATA)
        {
        }

        public void BuzzerOn (DeviceInfoService device)
        {
            device.writeCharacteristic('f000aa66-0451-4000-b000-000000000000', new Uint8Array([1]), function() {
                // Data: buzzer on.
                device.writeCharacteristic('f000aa65-0451-4000-b000-000000000000', new Uint8Array([4]), function() {
                    console.log("buzzer on success");
                }, app.buzzerFail() );
            }, app.buzzerFail() );


        }

        public void BuzzerOff ()
        {

        }
    }
}
