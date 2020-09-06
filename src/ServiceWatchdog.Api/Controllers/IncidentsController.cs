using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceWatchdog.Api.Controllers.RequestModels;
using ServiceWatchdog.Api.Models;
using ServiceWatchdog.Api.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace ServiceWatchdog.Api.Controllers
{
    [Authorize]
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

        [AllowAnonymous]
        [HttpGet]

        [SwaggerOperation(
            Summary = "Get all currently active incidents.",
            Description = "If an incident is currently not marked as resolved, this endpoint will return it."
        )]
        [SwaggerResponse(200, "", typeof(Incident[]))]
        public IActionResult GetActiveIncidents()
        {
            return Ok(_incidentsManager.GetAllActiveIncidents());
        }

        [AllowAnonymous]
        [HttpGet("recent")]

        [SwaggerOperation(
            Summary = "Get all recent incidents.",
            Description = "If an incident occurred in the last 5 days, it is returned by this endpoint."
        )]
        [SwaggerResponse(200, "", typeof(Incident[]))]
        public IActionResult GetRecentIncidents()
        {
            return Ok(_incidentsManager.GetRecentIncidents());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]

        [SwaggerOperation(
            Summary = "Get a specific incident by id.",
            Description = "All incidents are assigned a unique identifier, and this endpoint allows information about an incident to be retrieved on an incident-by-incident basis."
        )]
        [SwaggerResponse(200, "", typeof(Incident))]
        [SwaggerResponse(404, "Service was not found", typeof(Error))]
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

        [SwaggerOperation(
            Summary = "Edit incident properties here. If a property is set to null, it is ignored.",
            Description = "Change display information like the title or severity of an incident here. Note that once an incident state has been marked as resolved, this endpoint will cease to work."
        )]
        [SwaggerResponse(200, "", typeof(Incident))]
        [SwaggerResponse(404, "An incident with this ID does not exist.", typeof(Error))]
        [SwaggerResponse(400, "Resolved incidents cannot be edited.", typeof(Error))]
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

        [SwaggerOperation(
            Summary = "Add update information to an incident.",
            Description = "As the state of an incident changes, this endpoint allows updates to be appended to the incident view."
        )]
        [SwaggerResponse(404, "An incident with this ID does not exist.", typeof(Error))]
        [SwaggerResponse(400, "This incident has already been marked as resolved.", typeof(Error))]
        [SwaggerResponse(200, "", typeof(Incident))]
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
                incident.ResolvedAt = DateTime.UtcNow;

                var service = _servicesManager.GetService(incident.ServiceId);
                var incidents = _incidentsManager.GetIncidents(incident.ServiceId);
                var unresolvedIncidents = incidents.Where(x => x.ResolvedAt == null && x.Id != incident.Id);

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
