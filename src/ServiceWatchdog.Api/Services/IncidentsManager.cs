using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServiceWatchdog.Api.Models;
using ServiceWatchdog.Api.Services.Entities;

namespace ServiceWatchdog.Api.Services
{
    public class IncidentsManager
    {
        private readonly IConfiguration _config;
        private readonly ServicesManager _servicesManager;

        public IncidentsManager(IConfiguration config, ServicesManager servicesManager)
        {
            _config = config;
            _servicesManager = servicesManager;
        }

        public Incident CreateIncident(Incident incident)
        {
            var model = new IncidentModel(incident);

            using var ctx = CreateContext();

            ctx.Incidents.Add(model);
            ctx.SaveChanges();

            return new Incident(model);
        }

        public Incident GetIncident(int id)
        {
            using var ctx = CreateContext();

            var incident = ctx.Incidents.Include(x => x.Updates).FirstOrDefault(x => x.Id == id);
            if (incident == null)
                return null;
            return new Incident(incident);
        }

        public Incident UpdateIncident(Incident incident)
        {
            var model = new IncidentModel(incident);

            using var ctx = CreateContext();
            ctx.Incidents.Update(model);
            ctx.SaveChanges();

            return new Incident(model);
        }

        public IEnumerable<Incident> GetIncidents(int serviceId)
        {
            var service = _servicesManager.GetService(serviceId);
            if (service == null)
                return null;

            using var ctx = CreateContext();

            return ctx.Incidents.Include(x => x.Service).Where(x => x.Service.Id == serviceId)
                .ToArray()
                .Select(x => new Incident(x));
        }

        public IEnumerable<Incident> GetAllActiveIncidents()
        {
            using var ctx = CreateContext();

            return ctx.Incidents.Where(x => x.ResolvedAt == null).ToArray().Select(x => new Incident(x));
        }

        public IEnumerable<Incident> GetRecentIncidents(int withinDays = 5)
        {
            using var ctx = CreateContext();

            var now = DateTime.UtcNow.Date;
            var withinDate = now.AddDays(-withinDays);
            return ctx.Incidents.Where(x => x.ResolvedAt.HasValue && x.ResolvedAt.Value > withinDate).ToArray().Select(x => new Incident(x));
        }

        public void DeleteIncident(int id)
        {
            using var ctx = CreateContext();

            ctx.Incidents.Remove(new IncidentModel { Id = id });
            ctx.SaveChanges();
        }

        private WatchdogContext CreateContext()
        {
            return new WatchdogContext(_config);
        }
    }
}
