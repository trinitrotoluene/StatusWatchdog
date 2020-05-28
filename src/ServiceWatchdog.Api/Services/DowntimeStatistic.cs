using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ServiceWatchdog.Api.Services
{
    public class DowntimeStatistic
    {
        [JsonPropertyName("up")]
        public double UpPercentage { get; set; }

        [JsonPropertyName("outages")]
        public IEnumerable<OutageStatistic> Outages { get; set; }

        [JsonPropertyName("for")]
        public DateTime? ForDate { get; set; }
    }
}
