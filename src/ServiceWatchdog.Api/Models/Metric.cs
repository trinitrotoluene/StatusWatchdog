using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ServiceWatchdog.Api.Services.Entities;

namespace ServiceWatchdog.Api.Models
{
    public class Metric
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("displayed")]
        public bool Displayed { get; set; }

        [JsonPropertyName("service_id")]
        public int ServiceId { get; set; }

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
            Entries = model.Entries.Select(x => new MetricEntry(x));
        }
    }
}
