using System;
using System.Text.Json.Serialization;
using StatusWatchdog.Models;

namespace StatusWatchdog.Services
{
    public class OutageStatistic
    {
        [JsonPropertyName("type")]
        public ServiceStatus Type { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }
    }
}
