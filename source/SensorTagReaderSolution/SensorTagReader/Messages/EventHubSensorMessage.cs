using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X2CodingLab.SensorTag.Sensors;

namespace SensorTagReader.Messages
{
    public class EventHubSensorMessage
    {
        [JsonProperty("horseName")]
        public string HorseName { get; set; }

        [JsonProperty("sessionID")]
        public string SessionID { get; set; }

        [JsonProperty("sensorName")]
        public string SensorName { get; set; }

        [JsonProperty("sensorFriendlyName")]
        public string SensorFriendlyName { get; set; }

        [JsonProperty("sensorSystemID")]
        public string SensorSystemID { get; set; }

        [JsonProperty("time")]
        public DateTime TimeWhenRecorded { get; set; }

        [JsonIgnore]
        public double Temperature { get; set; }

        [JsonIgnore]
        public double Humidity { get; set; }

        [JsonProperty("temperature")]
        public double TemperatureTruncated
        {
            get { return Math.Round(Temperature, 2); }
        }

        [JsonProperty("humidity")]
        public double HumidityTruncated
        {
            get { return Math.Round(Humidity, 2); }
        }

        [JsonProperty("movement")]
        public Movement.MovementMeasurement Movement { get; set; }

    }
}
