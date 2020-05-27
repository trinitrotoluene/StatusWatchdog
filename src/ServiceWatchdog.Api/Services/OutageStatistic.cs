using System;
using System.Text.Json.Serialization;
using ServiceWatchdog.Api.Models;

namespace ServiceWatchdog.Api.Services
{
    public class OutageStatistic
    {
        [JsonPropertyName("type")]
        public ServiceStatus Type { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }
    }
}