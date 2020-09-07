using System.Text.Json.Serialization;
using StatusWatchdog.Services.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace StatusWatchdog.Models
{
    [SwaggerSchema("A metric entry. Associated with custom metrics to create a time series of arbitrary data.")]
    public class MetricEntry
    {
        [SwaggerSchema("The unique ID of the metric entry.")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [SwaggerSchema("The value of this metric entry.")]
        [JsonPropertyName("value")]
        public int Value { get; set; }

        [SwaggerSchema("The unique tag associated with this metric entry.")]
        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [SwaggerSchema("The ID of the custom metric that this entry is associated with.")]
        [JsonPropertyName("metric_id")]
        public int MetricId { get; set; }

        public MetricEntry()
        {
        }

        public MetricEntry(MetricEntryModel model)
        {
            Id = model.Id;
            Value = model.Value;
            Tag = model.Tag;
            MetricId = model.MetricId;
        }
    }
}
