using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ServiceWatchdog.Api.Controllers.RequestModels;
using ServiceWatchdog.Api.Services;

namespace ServiceWatchdog.Api.Controllers
{
    [Route("api/v1/services")]
    [ApiController]
    public class IndexController : Controller
    {
        private readonly DowntimeCalculator _downtimeCalculator;
        private readonly ServicesManager _servicesManager;
        private readonly IncidentsManager _incidentsManager;
        private readonly IncidentUpdatesManager _incidentUpdatesManager;

        public IndexController(IncidentsManager incidentsManager, IncidentUpdatesManager incidentUpdatesManager, ServicesManager servicesManager, DowntimeCalculator downtimeCalculator)
        {
            _incidentsManager = incidentsManager;
            _incidentUpdatesManager = incidentUpdatesManager;
            _servicesManager = servicesManager;
            _downtimeCalculator = downtimeCalculator;
        }

        [HttpGet]
        public IActionResult GetServices()
        {
            return Ok(_servicesManager.GetServices());
        }

        [HttpGet("{id}")]
        public IActionResult GetServiceById([FromRoute] int id)
        {
            var service = _servicesManager.GetService(id);

            if (service == null)
            {
                return NotFound(new
                {
                    message = "A service with this ID does not exist."
                });
            }

            return Ok(service);
        }

        [HttpPost]
        public IActionResult CreateService([FromBody] CreateServiceRequest requestBody)
        {
            var service = requestBody.ToService();
            if (_servicesManager.GetService(service.Slug) != null)
            {
                return BadRequest(new
                {
                    message = "A service with this slug already exists."
                });
            }

            service = _servicesManager.CreateService(service);
            return Ok(service);
        }

        [HttpGet("{id}/incidents")]
        public IActionResult GetIncidents([FromRoute] int id, [FromQuery] int days = 30)
        {
            var service = _servicesManager.GetService(id);
            if (service == null)
            {
                return NotFound(new
                {
                    message = "A service with this ID does not exist."
                });
            }

            var cutoffDate = DateTimeOffset.UtcNow.AddDays(days);
            return Ok(service.Incidents.Where(x => x.CreatedAt < cutoffDate));
        }

        [HttpPost("{id}/incidents")]
        public IActionResult CreateIncident([FromRoute] int id, [FromBody] CreateIncidentRequest requestBody)
        {
            var service = _servicesManager.GetService(id);
            if (service == null)
            {
                return NotFound(new
                {
                    message = "Service ID not found."
                });
            }

            var incident = requestBody.ToIncident();
            incident.ServiceId = service.Id;
            incident = _incidentsManager.CreateIncident(incident);

            var incidentUpdate = requestBody.ToIncidentUpdate();
            incidentUpdate.IncidentId = incident.Id;
            incidentUpdate = _incidentUpdatesManager.CreateIncidentUpdate(incidentUpdate);

            incident.MostRecentUpdateId = incidentUpdate.Id;
            _incidentsManager.UpdateIncident(incident);

            if (service.Status < incident.CausedStatus)
            {
                service.Status = incident.CausedStatus;
                _servicesManager.Update(service);
            }

            return Ok(incident);
        }

        [HttpGet("{id}/uptime")]
        public IActionResult GetServiceUptime([FromRoute] int id, [FromQuery] int limit = 60)
        {
            var service = _servicesManager.GetService(id);
            if (service == null)
            {
                return NotFound(new
                {
                    message = "Service ID not found."
                });
            }

            var uptimeStatistics = _downtimeCalculator.GetDowntimeStatistics(service, limit);

            return Ok(uptimeStatistics);
        }
    }
}
