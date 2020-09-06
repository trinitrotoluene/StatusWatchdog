using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ServiceWatchdog.Api.Controllers.RequestModels;
using ServiceWatchdog.Api.Models;
using ServiceWatchdog.Api.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace ServiceWatchdog.Api.Controllers
{
    [Route("api/v1/services")]
    [ApiController]
    public class ServicesController : Controller
    {
        private readonly DowntimeCalculator _downtimeCalculator;
        private readonly ServicesManager _servicesManager;
        private readonly IncidentsManager _incidentsManager;
        private readonly IncidentUpdatesManager _incidentUpdatesManager;
        private readonly MetricsManager _metricsManager;

        public ServicesController(IncidentsManager incidentsManager, IncidentUpdatesManager incidentUpdatesManager, ServicesManager servicesManager, DowntimeCalculator downtimeCalculator, MetricsManager metricsManager)
        {
            _incidentsManager = incidentsManager;
            _incidentUpdatesManager = incidentUpdatesManager;
            _servicesManager = servicesManager;
            _downtimeCalculator = downtimeCalculator;
            _metricsManager = metricsManager;
        }

        [HttpGet]

        [SwaggerOperation(
            Summary = "Get all services that the status page displays.",
            Description = "This endpoint returns limited information about a service, use the specific endpoints to get a service's incident history."
        )]
        [SwaggerResponse(200, "", typeof(Service[]))]
        public IActionResult GetServices()
        {
            return Ok(_servicesManager.GetServices());
        }

        [HttpGet("{id}")]

        [SwaggerOperation(
            Summary = "Retrieve more detailed information about a service by id.",
            Description = "This endpoint currently returns the entire incident history of a service. This may change in the future."
        )]
        [SwaggerResponse(200, "", typeof(Service))]
        [SwaggerResponse(404, "A service with this ID does not exist.", typeof(Error))]
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

        [SwaggerOperation(
            Summary = "Create a new service entry."
        )]
        [SwaggerResponse(200, "", typeof(Service))]
        [SwaggerResponse(400, "A service with this slug already exists.", typeof(Error))]
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

        [SwaggerOperation(
            Summary = "Get all incidents in the last N days by service id.",
            Description = "You can specify how far into the past this endpoint will search as a query parameter."
        )]
        [SwaggerResponse(200, "", typeof(Incident[]))]
        [SwaggerResponse(404, "A service with this ID does not exist.", typeof(Error))]
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

        [SwaggerOperation(
            Summary = "Create a new incident and update the status of a service by id.",
            Description = @"The status of the service will automatically be changed to that of the ""worst"" current incident."
        )]
        [SwaggerResponse(200, "", typeof(Incident))]
        [SwaggerResponse(404, "A service with this ID does not exist.", typeof(Error))]
        public IActionResult CreateIncident([FromRoute] int id, [FromBody] CreateIncidentRequest requestBody)
        {
            var service = _servicesManager.GetService(id);
            if (service == null)
            {
                return NotFound(new
                {
                    message = "A service with this ID does not exist."
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

        [SwaggerOperation(
            Summary = "Get the uptime metric of a service.",
            Description = "All services automatically have their historic uptime calculated. This operation is expensive for services with a long history, and therefore results returned by this endpoint are cached after the first request."
        )]
        [SwaggerResponse(200, "", typeof(DowntimeStatistic[]))]
        [SwaggerResponse(404, "A service with this ID does not exist.", typeof(Error))]
        public IActionResult GetServiceUptime([FromRoute] int id, [FromQuery] int limit = 60)
        {
            var service = _servicesManager.GetService(id);
            if (service == null)
            {
                return NotFound(new
                {
                    message = "A service with this ID does not exist."
                });
            }

            var uptimeStatistics = _downtimeCalculator.GetDowntimeStatistics(service, limit);

            return Ok(uptimeStatistics);
        }

        [HttpGet("{id}/metrics")]

        [SwaggerOperation(
            Summary = "Get shallow information about all custom metrics associated with a service."
        )]
        [SwaggerResponse(200, "", typeof(Metric[]))]
        public IActionResult GetMetrics([FromRoute] int id)
        {
            var metrics = _metricsManager.GetMetrics(id);
            return Ok(metrics);
        }

        [HttpPost("{id}/metrics")]

        [SwaggerOperation(
            Summary = "Add a new custom metric to the service."
        )]
        [SwaggerResponse(200, "", typeof(Metric))]
        [SwaggerResponse(404, "A service with this ID does not exist.", typeof(Error))]
        public IActionResult CreateMetric([FromRoute] int id, [FromBody] CreateMetricRequest requestBody)
        {
            var service = _servicesManager.GetService(id);
            if (service == null)
            {
                return NotFound(new
                {
                    message = "A service with this ID does not exist."
                });
            }

            var metric = requestBody.ToMetric();
            metric.ServiceId = id;
            metric = _metricsManager.CreateMetric(metric);

            return Ok(metric);
        }
    }
}
