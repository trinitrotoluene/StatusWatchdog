using System.Text.Json.Serialization;
using Swashbuckle.AspNetCore.Annotations;

namespace ServiceWatchdog.Api.Models
{
    [SwaggerSchema("The base error entity, providing metadata to let you know why an operation failed.")]
    public class Error
    {
        [SwaggerSchema("The error message.")]
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
