using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using StatusWatchdog.Models;
using StatusWatchdog.Services.Entities;

namespace StatusWatchdog.Services
{
    public class MetricsManager
    {
        private readonly IConfiguration _config;

        public MetricsManager(IConfiguration config)
        {
            _config = config;
        }

        public Metric GetMetric(int metricId)
        {
            using var ctx = CreateContext();
            var metric = ctx.Metrics.FirstOrDefault(x => x.Id == metricId);
            if (metric == null)
                return null;

            return new Metric(metric);
        }

        public IEnumerable<Metric> GetMetrics(int serviceId)
        {
            using var ctx = CreateContext();
            return ctx.Metrics.Where(x => x.ServiceId == serviceId && x.Displayed).ToArray().Select(x => new Metric(x));
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

        public IEnumerable<MetricEntry> GetEntries(int metricId, int limit)
        {
            using var ctx = CreateContext();
            var entries = ctx.MetricEntries.Where(x => x.MetricId == metricId)
                .OrderByDescending(x => x.Id)
                .Take(limit)
                .ToArray()
                .Select(x => new MetricEntry(x))
                .Reverse();

            return entries;
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
