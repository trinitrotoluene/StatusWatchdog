using StatusWatchdog.Models;

namespace StatusWatchdog.Services.Entities
{
    public class MetricEntryModel
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public string Tag { get; set; }

        public int MetricId { get; set; }

        public MetricModel Metric { get; set; }

        public MetricEntryModel()
        {
        }

        public MetricEntryModel(MetricEntry entry)
        {
            Id = entry.Id;
            Value = entry.Value;
            Tag = entry.Tag;
            MetricId = entry.MetricId;
        }
    }
}
