using System;
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
                incident.ResolvedAt = DateTimeOffset.UtcNow;

            _incidentsManager.UpdateIncident(incident);

            return Ok(_incidentsManager.GetIncident(id));
        }
    }
}