using System;
using System.Collections.Generic;
using ServiceWatchdog.Api.Models;

namespace ServiceWatchdog.Api.Services
{
    public struct DowntimeWindow
        {
            private DateTimeOffset _start;
            private DateTimeOffset _end;
            private List<OutageStatistic> _outages;

            public DowntimeWindow(DateTimeOffset start, DateTimeOffset end, ServiceStatus causedStatus)
            {
                _start = start;
                _end = end;
                _outages = new List<OutageStatistic>();

                CreateStatistic(end - start, causedStatus);
            }

            public DateTimeOffset Start => _start;

            public DateTimeOffset End => _end;

            public TimeSpan Duration => _end - _start;

            public List<OutageStatistic> Outages => _outages;

            public bool OverlapsWith(DateTimeOffset start, DateTimeOffset end)
            {
                if (start > _start && start < _end)
                {
                    return true;
                }

                if (start < _start && end > _start)
                {
                    return true;
                }

                return false;
            }

            public void Adjust(DateTimeOffset start, DateTimeOffset end, ServiceStatus causedStatus)
            {
                if (start < _start)
                {
                    _start = start;
                }

                if (end > _end)
                {
                    _end = end;
                }

                CreateStatistic(end - start, causedStatus);
            }
            
            private void CreateStatistic(TimeSpan duration, ServiceStatus status)
            {
                var outageStatistic = new OutageStatistic
                {
                    Type = status,
                    Duration = (int)(duration.TotalSeconds)
                };

                _outages.Add(outageStatistic);
            }
        }
}