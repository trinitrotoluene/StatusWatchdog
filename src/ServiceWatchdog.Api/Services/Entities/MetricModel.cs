using System.Collections.Generic;
using ServiceWatchdog.Api.Models;

namespace ServiceWatchdog.Api.Services.Entities
{
    public class MetricModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Displayed { get; set; }

        public int ServiceId { get; set; }

        public ServiceModel Service { get; set; }

        public ICollection<MetricEntryModel> Entries { get; set; }

        public MetricModel()
        {
        }

        public MetricModel(Metric metric)
        {
            Id = metric.Id;
            Name = metric.Name;
            Displayed = metric.Displayed;
            ServiceId = metric.ServiceId;
        }
    }
}
