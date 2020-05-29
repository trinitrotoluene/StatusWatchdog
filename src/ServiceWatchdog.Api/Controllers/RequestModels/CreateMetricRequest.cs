using System.Text.Json.Serialization;
using ServiceWatchdog.Api.Models;

namespace ServiceWatchdog.Api.Controllers.RequestModels
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
