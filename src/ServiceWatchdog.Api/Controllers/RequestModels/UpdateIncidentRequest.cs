using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ServiceWatchdog.Api.Models;

namespace ServiceWatchdog.Api.Controllers.RequestModels
{
    public class UpdateIncidentRequest
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("caused_status")]
        public ServiceStatus? CausedStatus { get; set; }
    }
}