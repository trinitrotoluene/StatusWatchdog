namespace ServiceWatchdog.Api.Services.Entities
{
    public class MetricEntryModel
    {
        public int Id { get; set; }

        public int Value { get; set; }

        public int Tag { get; set; }

        public int MetricId { get; set; }

        public MetricModel Metric { get; set; }
    }
}
