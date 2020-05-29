using System.Linq;
using Microsoft.Extensions.Configuration;
using ServiceWatchdog.Api.Models;
using ServiceWatchdog.Api.Services.Entities;

namespace ServiceWatchdog.Api.Services
{
    public class MetricsManager
    {
        private readonly IConfiguration _config;

        public MetricsManager(IConfiguration config)
        {
            _config = config;
        }

        public Metric CreateMetric(Metric metric)
        {
            using var ctx = CreateContext();
            var model = new MetricModel(metric);

            ctx.Metrics.Add(model);
            ctx.SaveChanges();

            return new Metric(model);
        }

        public void RemoveMetric(int metricId)
        {
            using var ctx = CreateContext();
            ctx.Metrics.Remove(new MetricModel { Id = metricId });
            ctx.SaveChanges();
        }

        public MetricEntry CreateEntry(MetricEntry entry)
        {
            using var ctx = CreateContext();
            var model = new MetricEntryModel(entry);

            ctx.MetricEntries.Add(model);
            ctx.SaveChanges();

            return new MetricEntry(model);
        }

        public void ClearEntries(int metricId)
        {
            using var ctx = CreateContext();
            ctx.MetricEntries.RemoveRange(ctx.MetricEntries.Where(x => x.MetricId == metricId));
            ctx.SaveChanges();
        }

        private WatchdogContext CreateContext()
        {
            return new WatchdogContext(_config);
        }
    }
}
