using System;
using System.Collections.Generic;
using StatusWatchdog.Models;

namespace StatusWatchdog.Services.Entities
{
    public class IncidentModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ResolvedAt { get; set; }

        public IncidentState State { get; set; }

        public ServiceStatus CausedStatus { get; set; }

        public int MostRecentUpdateId { get; set; }

        public int ServiceId { get; set; }

        public ServiceModel Service { get; set; }

        public ICollection<IncidentUpdateModel> Updates { get; set; }

        public IncidentModel()
        {
        }

        public IncidentModel(Incident incident)
        {
            Id = incident.Id;
            Title = incident.Title;
            CreatedAt = incident.CreatedAt;
            ResolvedAt = incident.ResolvedAt;
            State = incident.State;
            CausedStatus = incident.CausedStatus;
            MostRecentUpdateId = incident.MostRecentUpdateId;
            ServiceId = incident.ServiceId;
        }
    }
}
