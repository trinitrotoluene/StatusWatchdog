using System.Text.Json.Serialization;
using ServiceWatchdog.Api.Services.Entities;

namespace ServiceWatchdog.Api.Models
{
    public class MetricEntry
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }


        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("tag")]
        public int Tag { get; set; }

        [JsonPropertyName("metric_id")]
        public int MetricId { get; set; }

        public MetricEntry()
        {
        }

        public MetricEntry(MetricEntryModel model)
        {
            Id = model.Id;
            Value = model.Id;
            Tag = model.Tag;
            MetricId = model.MetricId;
        }
    }
}
