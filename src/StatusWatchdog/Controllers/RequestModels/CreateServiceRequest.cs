using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using StatusWatchdog.Models;

namespace StatusWatchdog.Controllers.RequestModels
{
    public class CreateServiceRequest
    {
        [Required]
        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [Required]
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [Required]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        public Service ToService()
        {
            return new Service
            {
                Slug = Slug,
                DisplayName = DisplayName,
                Description = Description
            };
        }
    }
}
