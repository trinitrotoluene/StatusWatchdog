using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ServiceWatchdog.Api.Models;

namespace ServiceWatchdog.Api.Controllers.RequestModels
{
    public class CreateIncidentRequest
    {
        [Required]
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [Required]
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [Required]
        [JsonPropertyName("state")]
        public IncidentState State { get; set; }

        public Incident ToIncident()
        {
            return new Incident
            {
                State = State,
                Title = Title
            };
        }

        public IncidentUpdate ToIncidentUpdate()
        {
            return new IncidentUpdate
            {
                State = State,
                Message = Message
            };
        }
    }
}