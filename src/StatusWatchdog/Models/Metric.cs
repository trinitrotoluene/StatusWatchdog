using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using StatusWatchdog.Services.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace StatusWatchdog.Models
{
    [SwaggerSchema("A custom metric. Used to store arbitrary additonal information associated with a service in a time series.")]
    public class Metric
    {
        [SwaggerSchema("The unique ID of the custom metric.")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [SwaggerSchema("The display name of a custom metric.")]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [SwaggerSchema("Whether the metric is displayed underneath the service or not.")]
        [JsonPropertyName("displayed")]
        public bool Displayed { get; set; }

        [SwaggerSchema("The ID of the service associated with this custom metric.")]
        [JsonPropertyName("service_id")]
        public int ServiceId { get; set; }

        [SwaggerSchema("Optional field containing data pushed to this custom metric. Null except when implicitly returned by the function of a metric endpoint.")]
        [JsonPropertyName("entries")]
        public IEnumerable<MetricEntry> Entries { get; set; }

        public Metric()
        {
        }

        public Metric(MetricModel model)
        {
            Id = model.Id;
            Name = model.Name;
            Displayed = model.Displayed;
            ServiceId = model.ServiceId;
            Entries = model.Entries?.Select(x => new MetricEntry(x));
        }
    }
}
