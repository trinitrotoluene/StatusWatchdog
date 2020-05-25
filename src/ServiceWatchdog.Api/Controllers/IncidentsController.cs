using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ServiceWatchdog.Api.Controllers.RequestModels;
using ServiceWatchdog.Api.Models;
using ServiceWatchdog.Api.Services;

namespace ServiceWatchdog.Api.Controllers
{
    [Route("api/v1/incidents")]
    [ApiController]
    public class IncidentsController : Controller
    {
        private readonly ServicesManager _servicesManager;
        private readonly IncidentsManager _incidentsManager;
        private readonly IncidentUpdatesManager _incidentUpdatesManager;

        public IncidentsController(IncidentsManager incidentsManager, IncidentUpdatesManager incidentUpdatesManager, ServicesManager servicesManager)
        {
            _incidentsManager = incidentsManager;
            _incidentUpdatesManager = incidentUpdatesManager;
            _servicesManager = servicesManager;
        }

        [HttpGet("{id}")]
        public IActionResult GetIncident([FromRoute] int id)
        {
            var incident = _incidentsManager.GetIncident(id);
            if (incident == null)
            {
                return NotFound(new
                {
                    message = "An incident with this ID does not exist."
                });
            }

            return Ok(incident);
        }

        [HttpPatch("{id}")]
        public IActionResult UpdateIncident([FromRoute] int id, [FromBody] UpdateIncidentRequest requestBody)
        {
            var incident = _incidentsManager.GetIncident(id);
            if (incident == null)
            {
                return NotFound(new
                {
                    message = "An incident with this ID does not exist."
                });
            }

            if (incident.State == IncidentState.Resolved)
            {
                return BadRequest(new
                {
                    message = "You may not edit a resolved incident."
                });
            }

            incident.Title = requestBody?.Title ?? incident.Title; ;
            incident.CausedStatus = requestBody?.CausedStatus ?? incident.CausedStatus;
            _incidentsManager.UpdateIncident(incident);

            var service = _servicesManager.GetService(incident.ServiceId);
            if (service.Status < incident.CausedStatus)
            {
                service.Status = incident.CausedStatus;
                _servicesManager.Update(service);
            }

            return Ok(incident);
        }

        [HttpPost("{id}/updates")]
        public IActionResult AddUpdate([FromRoute] int id, [FromBody] CreateUpdateRequest requestBody)
        {
            var incident = _incidentsManager.GetIncident(id);
            if (incident == null)
            {
                return NotFound(new
                {
                    message = "An incident with this ID does not exist."
                });
            }

            var update = requestBody.ToIncidentUpdate();

            if (incident.State == IncidentState.Resolved)
            {
                return BadRequest(new
                {
                    message = "This incident has already been marked as resolved."
                });
            }

            update.IncidentId = incident.Id;
            update = _incidentUpdatesManager.CreateIncidentUpdate(update);

            incident.State = update.State;
            incident.MostRecentUpdateId = update.Id;
            if (incident.State == IncidentState.Resolved)
            {
                incident.ResolvedAt = DateTimeOffset.UtcNow;

                var service = _servicesManager.GetService(incident.ServiceId);
                var incidents = _incidentsManager.GetIncidents(incident.ServiceId);
                var unresolvedIncidents = incidents.Where(x => x.ResolvedAt == null);

                if (!unresolvedIncidents.Any())
                {
                    service.Status = ServiceStatus.Normal;
                    _servicesManager.Update(service);
                }
                else
                {
                    var nextStatus = unresolvedIncidents
                        .OrderByDescending(x => x.CausedStatus)
                        .First()
                        .CausedStatus;
                    service.Status = nextStatus;
                    _servicesManager.Update(service);
                }
            }

            _incidentsManager.UpdateIncident(incident);

            return Ok(_incidentsManager.GetIncident(id));
        }
    }
}