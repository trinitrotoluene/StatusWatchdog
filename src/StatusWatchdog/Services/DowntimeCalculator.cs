using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using StatusWatchdog.Models;

namespace StatusWatchdog.Services
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
            var now = DateTime.UtcNow.Date;

            var serviceAge = now - service.CreatedAt.Date;
            var availableDaysOfData = serviceAge.TotalDays + 1;

            int offset = 0;
            if (availableDaysOfData < limit) {
                for (; offset < limit - availableDaysOfData; offset++) {
                    statistics[offset] = new DowntimeStatistic { UpPercentage = -1, Outages = null, ForDate = null };
                }
            }


            for (; offset < limit; offset++)
            {
                var date = now.AddDays((offset - limit) + 1);
                if (offset == limit - 1)
                    statistics[offset] = GetPercentageUptime(service.Id, date, DateTime.UtcNow);
                else
                    statistics[offset] = GetPercentageUptime(service.Id, date);
            }

            _memoryCache.CreateEntry(DOWNTIME_CACHE_PREFIX + service.Id.ToString()).SetAbsoluteExpiration(TimeSpan.FromSeconds(5));

            return statistics;
        }

        private DowntimeStatistic GetPercentageUptime(int serviceId, DateTime date, DateTimeOffset? nowTime = null)
        {
            DateTime now;
            if (nowTime == null)
            {
                now = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 0);
            }
            else
            {
                now = nowTime.Value.UtcDateTime;
            }

            var incidents = _incidentsManager.GetIncidents(serviceId);
            var applicableIncidents = incidents.Where(x =>
            {
                if (x.CreatedAt.Date < now.Date && (x.ResolvedAt == null || x.ResolvedAt.Value.Date >= now.Date))
                    return true;

                if (x.CreatedAt.Date == now.Date)
                    return true;

                return false;
            })
            .ToArray();

            if (applicableIncidents.Length == 0)
                return new DowntimeStatistic { UpPercentage = 100, Outages = null };

            var windows = new List<DowntimeWindow>();
            foreach (var incident in applicableIncidents)
            {
                if (incident.CreatedAt < date && incident.ResolvedAt.HasValue && incident.ResolvedAt > now)
                {
                    ComputeIntoWindows(windows, date, now, incident.CausedStatus);
                }
                else if (incident.CreatedAt < date)
                {
                    ComputeIntoWindows(windows, date, incident.ResolvedAt ?? now, incident.CausedStatus);
                }
                else if (incident.ResolvedAt > now)
                {
                    ComputeIntoWindows(windows, incident.CreatedAt, now, incident.CausedStatus);
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
            return new DowntimeStatistic { UpPercentage = upPercentage, Outages = outages, ForDate = date };
        }

        private void ComputeIntoWindows(List<DowntimeWindow> windows, DateTime createdAt, DateTime resolvedAt, ServiceStatus causedStatus)
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
