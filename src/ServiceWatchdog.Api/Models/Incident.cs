using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ServiceWatchdog.Api.Services.Entities;

namespace ServiceWatchdog.Api.Models
{
    public class Incident
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("resolved_at")]
        public DateTime? ResolvedAt { get; set; }

        [JsonPropertyName("state")]
        public IncidentState State { get; set; }

        [JsonPropertyName("caused_status")]
        public ServiceStatus CausedStatus { get; set; }

        [JsonPropertyName("most_recent_update_id")]
        public int MostRecentUpdateId { get; set; }

        [JsonPropertyName("service_id")]
        public int ServiceId { get; set; }

        [JsonPropertyName("updates")]
        public IEnumerable<IncidentUpdate> Updates { get; set; }

        public Incident()
        {
        }

        public Incident(IncidentModel model)
        {
            Id = model.Id;
            Title = model.Title;
            CreatedAt = model.CreatedAt;
            ResolvedAt = model.ResolvedAt;
            State = model.State;
            CausedStatus = model.CausedStatus;
            MostRecentUpdateId = model.MostRecentUpdateId;
            ServiceId = model.ServiceId;
            Updates = model.Updates?.Select(x => new IncidentUpdate(x));
        }
    }
}