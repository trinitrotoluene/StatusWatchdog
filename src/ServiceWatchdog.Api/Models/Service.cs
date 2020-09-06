using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ServiceWatchdog.Api.Services.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace ServiceWatchdog.Api.Models
{
    [SwaggerSchema("A service. The base entity of the API.")]
    public class Service
    {
        [SwaggerSchema("The unique ID of the service.")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [SwaggerSchema("The slug of the service, used when uniquely linking to it.")]
        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [SwaggerSchema("The displayed name of the service.")]
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [SwaggerSchema("The displayed description of the service.")]
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [SwaggerSchema("The current status of this service.")]
        [JsonPropertyName("status")]
        public ServiceStatus Status { get; set; }

        [SwaggerSchema("The date and time the service was created.")]
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [SwaggerSchema("Optional field including incidents associated with the service. Typically null except when otherwise specified by an endpoint.")]
        [JsonPropertyName("incidents")]
        public IEnumerable<Incident> Incidents { get; set; }

        public Service()
        {
        }

        public Service(ServiceModel model)
        {
            Id = model.Id;
            Slug = model.Slug;
            DisplayName = model.DisplayName;
            Description = model.Description;
            Status = model.Status;
            CreatedAt = model.CreatedAt;
            Incidents = model.Incidents?.Select(x => new Incident(x));
        }
    }
}
