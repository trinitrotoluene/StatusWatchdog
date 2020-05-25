using System;
using System.Text.Json.Serialization;
using ServiceWatchdog.Api.Services.Entities;

namespace ServiceWatchdog.Api.Models
{
    public class IncidentUpdate
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("state")]
        public IncidentState State { get; set; }

        [JsonPropertyName("incident_id")]
        public int IncidentId { get; set; }

        public IncidentUpdate()
        {
        }

        public IncidentUpdate(IncidentUpdateModel model)
        {
            Id = model.Id;
            Message = model.Message;
            CreatedAt = model.CreatedAt;
            State = model.State;
            IncidentId = model.IncidentId;
        }
    }
}