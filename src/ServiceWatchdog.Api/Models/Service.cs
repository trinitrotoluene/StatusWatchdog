using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ServiceWatchdog.Api.Services.Entities;

namespace ServiceWatchdog.Api.Models
{
    public class Service
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("status")]
        public ServiceStatus Status { get; set; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

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