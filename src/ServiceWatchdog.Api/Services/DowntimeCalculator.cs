using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using ServiceWatchdog.Api.Models;

namespace ServiceWatchdog.Api.Services
{
    public class DowntimeCalculator
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ServicesManager _servicesManager;
        private readonly IncidentsManager _incidentsManager;

        public DowntimeCalculator(IMemoryCache memoryCache, IncidentsManager incidentsManager, ServicesManager servicesManager)
        {
            _memoryCache = memoryCache;
            _incidentsManager = incidentsManager;
            _servicesManager = servicesManager;
        }

        public DowntimeStatistic[] GetDowntimeStatistics(Service service, int limit = 60)
        {
            const string DOWNTIME_CACHE_PREFIX = "_service_downtime_aggregate:";

            if (_memoryCache.TryGetValue(DOWNTIME_CACHE_PREFIX + service.Id.ToString(), out var existingStatistic))
            {
                return (DowntimeStatistic[])existingStatistic;
            }

            var statistics = new DowntimeStatistic[limit];

            var oldestDate = DateTimeOffset.UtcNow.AddDays(-limit);
            var dayDiff = Math.Ceiling(service.CreatedAt.Subtract(oldestDate).TotalDays);

            int i = 0;
            for (; i < dayDiff && i < limit; i++)
            {
                statistics[i] = new DowntimeStatistic { UpPercentage = -1, Outages = null };
            }

            var now = DateTimeOffset.UtcNow.Date;

            for (; i < limit; i++)
            {
                var date = now.AddDays((i - limit) + 1);
                if (i == limit - 1)
                    statistics[i] = GetPercentageUptime(service.Id, date, DateTimeOffset.UtcNow);
                else
                    statistics[i] = GetPercentageUptime(service.Id, date);
            }

            _memoryCache.CreateEntry(DOWNTIME_CACHE_PREFIX + service.Id.ToString()).SetAbsoluteExpiration(TimeSpan.FromSeconds(5));

            return statistics;
        }

        private DowntimeStatistic GetPercentageUptime(int serviceId, DateTime date, DateTimeOffset? nowTime = null)
        {
            DateTimeOffset now;
            if (nowTime == null)
            {
                now = new DateTimeOffset(date.Year, date.Month, date.Day, 23, 59, 59, 0, new TimeSpan(0, 0, 0));
            }
            else
            {
                now = nowTime.Value;
            }

            var incidents = _incidentsManager.GetIncidents(serviceId);
            var applicableIncidents = incidents.Where(x =>
            {
                if (x.CreatedAt.Date < now.Date && x.ResolvedAt == null)
                    return true;

                if (x.CreatedAt.Date == now.Date)
                    return true;

                return false;
            })
            .ToArray();

            if (applicableIncidents.Length == 0)
                return new DowntimeStatistic { UpPercentage = 100, Outages = null };

            var windows = new List<DowntimeWindow>();
            var startOfDay = new DateTimeOffset(now.Date);
            foreach (var incident in applicableIncidents)
            {
                if (incident.CreatedAt < startOfDay)
                {
                    ComputeIntoWindows(windows, startOfDay, incident.ResolvedAt ?? now, incident.CausedStatus);
                }
                else
                {
                    ComputeIntoWindows(windows, incident.CreatedAt, incident.ResolvedAt ?? now, incident.CausedStatus);
                }
            }

            var downtimeWindowTotalSize = windows.Sum(x => Math.Abs(x.Duration.TotalSeconds));

            const int SECONDS_IN_DAY = 86400;
            double upPercentage = ((SECONDS_IN_DAY - downtimeWindowTotalSize) / SECONDS_IN_DAY) * 100;

            var outages = windows.SelectMany(x => x.Outages);
            return new DowntimeStatistic { UpPercentage = upPercentage, Outages = outages };
        }

        private void ComputeIntoWindows(List<DowntimeWindow> windows, DateTimeOffset createdAt, DateTimeOffset resolvedAt, ServiceStatus causedStatus)
        {
            foreach (var window in windows)
            {
                if (window.OverlapsWith(createdAt, resolvedAt))
                {
                    window.Adjust(createdAt, resolvedAt, causedStatus);
                    return;
                }
            }

            windows.Add(new DowntimeWindow(createdAt, resolvedAt, causedStatus));
        }
    }
}