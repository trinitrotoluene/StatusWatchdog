using System;
using ServiceWatchdog.Api.Models;

namespace ServiceWatchdog.Api.Services.Entities
{
    public class IncidentUpdateModel
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; }

        public IncidentState State { get; set; }

        public int IncidentId { get; set; }

        public IncidentModel Incident { get; set; }

        public IncidentUpdateModel()
        {
        }

        public IncidentUpdateModel(IncidentUpdate update)
        {
            Id = update.Id;
            Message = update.Message;
            CreatedAt = update.CreatedAt;
            State = update.State;
            IncidentId = update.IncidentId;
        }
    }
}
