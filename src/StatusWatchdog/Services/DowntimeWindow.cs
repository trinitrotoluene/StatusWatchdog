using System;
using System.Collections.Generic;
using StatusWatchdog.Models;

namespace StatusWatchdog.Services
{
    public struct DowntimeWindow
        {
            private DateTime _start;
            private DateTime _end;
            private List<OutageStatistic> _outages;

            public DowntimeWindow(DateTime start, DateTime end, ServiceStatus causedStatus)
            {
                _start = start;
                _end = end;
                _outages = new List<OutageStatistic>();

                CreateStatistic(end - start, causedStatus);
            }

            public DateTime Start => _start;

            public DateTime End => _end;

            public TimeSpan Duration => _end - _start;

            public List<OutageStatistic> Outages => _outages;

            public bool OverlapsWith(DateTime start, DateTime end)
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

            public void Adjust(DateTime start, DateTime end, ServiceStatus causedStatus)
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
