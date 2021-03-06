using System;
using System.Collections.Generic;
using StatusWatchdog.Models;

namespace StatusWatchdog.Services.Entities
{
    public class ServiceModel
    {
        public int Id { get; set; }

        public string Slug { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public ServiceStatus Status { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public ICollection<IncidentModel> Incidents { get; set; }

        public ICollection<MetricModel> Metrics { get; set; }

        public ServiceModel()
        {
        }

        public ServiceModel(Service service)
        {
            Id = service.Id;
            Slug = service.Slug;
            DisplayName = service.DisplayName;
            Description = service.Description;
            Status = service.Status;
            CreatedAt = service.CreatedAt;
        }
    }
}
