using System.Text.Json.Serialization;
using StatusWatchdog.Models;

namespace StatusWatchdog.Controllers.RequestModels
{
    public class CreateMetricEntryRequest
    {
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }

        public MetricEntry ToEntry()
        {
            return new MetricEntry
            {
                Tag = Tag,
                Value = Value
            };
        }
    }
}
