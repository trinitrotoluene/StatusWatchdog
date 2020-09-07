using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using StatusWatchdog.Models;

namespace StatusWatchdog.Controllers.RequestModels
{
    public class CreateUpdateRequest
    {
        [Required]
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [Required, RequiredEnum]
        [JsonPropertyName("state")]
        public IncidentState State { get; set; }

        public IncidentUpdate ToIncidentUpdate()
        {
            return new IncidentUpdate
            {
                Message = Message,
                State = State
            };
        }
    }
}
