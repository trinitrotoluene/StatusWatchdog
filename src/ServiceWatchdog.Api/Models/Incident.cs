using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ServiceWatchdog.Api.Services.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace ServiceWatchdog.Api.Models
{
    [SwaggerSchema("A service incident. Created to indicate that something has gone wrong.")]
    public class Incident
    {
        [SwaggerSchema("The unique ID of the incident.")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [SwaggerSchema("The displayed title of the incident.")]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [SwaggerSchema("The date and time that the incident was created.")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [SwaggerSchema("The date and time that the incident was marked as resolved.")]
        [JsonPropertyName("resolved_at")]
        public DateTime? ResolvedAt { get; set; }

        [SwaggerSchema("The current state of the incident.")]
        [JsonPropertyName("state")]
        public IncidentState State { get; set; }

        [SwaggerSchema("The effect that this incident has on its associated service.")]
        [JsonPropertyName("caused_status")]
        public ServiceStatus CausedStatus { get; set; }

        [SwaggerSchema("The ID of the most recently posted incident update.")]
        [JsonPropertyName("most_recent_update_id")]
        public int MostRecentUpdateId { get; set; }

        [SwaggerSchema("The ID of the service associated with this incident.")]
        [JsonPropertyName("service_id")]
        public int ServiceId { get; set; }

        [SwaggerSchema("Optional field that includes updates posted to the incident. Not returned on bulk incident endpoints by default.")]
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
