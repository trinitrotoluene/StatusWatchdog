using System;
using System.Text.Json.Serialization;
using StatusWatchdog.Services.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace StatusWatchdog.Models
{
    [SwaggerSchema("An incident update. Created whenever the status of an incident is updated with additional information.")]
    public class IncidentUpdate
    {
        [SwaggerSchema("The unique ID of an incident update.")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [SwaggerSchema("The message displayed along with the update.")]
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [SwaggerSchema("The date and time the update was created.")]
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        [SwaggerSchema("The state that the update changes the incident to.")]
        [JsonPropertyName("state")]
        public IncidentState State { get; set; }

        [SwaggerSchema("The ID of the incident that this update is associated with.")]
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
