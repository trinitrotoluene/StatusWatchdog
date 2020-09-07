using System.Text.Json.Serialization;
using StatusWatchdog.Models;

namespace StatusWatchdog.Controllers.RequestModels
{
    public class CreateMetricRequest
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        public Metric ToMetric()
        {
            return new Metric
            {
                Name = Name
            };
        }
    }
}
