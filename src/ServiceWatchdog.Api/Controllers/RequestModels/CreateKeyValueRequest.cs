using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServiceWatchdog.Api.Controllers.RequestModels
{
    public class CreateKeyValueRequest
    {
        [Required]
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [Required]
        [JsonPropertyName("value")]
        [MaxLength(8192)]
        public string Value { get; set; }
    }
}