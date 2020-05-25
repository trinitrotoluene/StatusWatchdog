using System;
using System.Collections.Generic;
using ServiceWatchdog.Api.Models;

namespace ServiceWatchdog.Api.Services.Entities
{
    public class ServiceModel
    {
        public int Id { get; set; }

        public string Slug { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public ServiceStatus Status { get; set; }

        public DateTimeOffset CreatedAt {get; set;}

        public ICollection<IncidentModel> Incidents { get; set; }

        public ServiceModel()
        {
        }

        public ServiceModel(Service service)
        {
            Id = service.Id;
            Slug = service.Slug;
            DisplayName = service.DisplayName;
            Description = service.Description;
            Status = ServiceStatus.Normal;
            CreatedAt = service.CreatedAt;
        }
    }
}