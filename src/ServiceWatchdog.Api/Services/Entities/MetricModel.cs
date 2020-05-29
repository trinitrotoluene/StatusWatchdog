using System.Collections.Generic;

namespace ServiceWatchdog.Api.Services.Entities
{
    public class MetricModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ServiceId { get; set; }

        public ServiceModel Service { get; set; }

        public ICollection<MetricEntryModel> Entries { get; set; }
    }
}
