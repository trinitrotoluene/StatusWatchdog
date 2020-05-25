using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServiceWatchdog.Api.Models;
using ServiceWatchdog.Api.Services.Entities;

namespace ServiceWatchdog.Api.Services
{
    public class IncidentUpdatesManager
    {
        private readonly IConfiguration _config;
        private readonly IncidentsManager _incidentsManager;

        public IncidentUpdatesManager(IConfiguration config, IncidentsManager incidentsManager)
        {
            _config = config;
            _incidentsManager = incidentsManager;
        }

        public IncidentUpdate CreateIncidentUpdate(IncidentUpdate incidentUpdate)
        {
            var model = new IncidentUpdateModel(incidentUpdate);
            
            using var ctx = CreateContext();

            ctx.IncidentUpdates.Add(model);
            ctx.SaveChanges();

            return new IncidentUpdate(model);
        }

        public IncidentUpdate GetIncidentUpdate(int id)
        {
            using var ctx = CreateContext();

            var incident = ctx.IncidentUpdates.FirstOrDefault(x => x.Id == id);
            if (incident == null)
                return null;
            return new IncidentUpdate(incident);
        }

        public IncidentUpdate UpdateIncidentUpdate(IncidentUpdate incidentUpdate)
        {
            var model = new IncidentUpdateModel(incidentUpdate);
            
            using var ctx = CreateContext();
            ctx.IncidentUpdates.Update(model);
            ctx.SaveChanges();

            return new IncidentUpdate(model);
        }

        public void DeleteIncidentUpdate(int id)
        {
            using var ctx = CreateContext();

            ctx.IncidentUpdates.Remove(new IncidentUpdateModel { Id = id });
            ctx.SaveChanges();
        }

        private WatchdogContext CreateContext()
        {
            return new WatchdogContext(_config);
        }
    }
}