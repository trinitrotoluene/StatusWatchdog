using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceWatchdog.Api.Controllers.RequestModels;
using ServiceWatchdog.Api.Models;
using ServiceWatchdog.Api.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace ServiceWatchdog.Api.Controllers
{
    [Authorize]
    [Route("api/v1/metrics")]
    [ApiController]
    public class MetricsController : Controller
    {
        private readonly MetricsManager _metricsManager;

        public MetricsController(MetricsManager metricsManager)
        {
            _metricsManager = metricsManager;
        }

        [AllowAnonymous]
        [HttpGet("{id}")]

        [SwaggerOperation(
            Summary = "Retrieve entries from a metric by id.",
            Description = "If you have a custom metric, it can be retrieved from this endpoint by id. By default the 60 most recent entries are returned."
        )]
        [SwaggerResponse(200, "", typeof(MetricEntry[]))]
        [SwaggerResponse(404, "A metric with this ID does not exist.", typeof(Error))]
        public IActionResult GetMetricEntries([FromRoute] int id, [FromQuery] int limit = 60)
        {
            if (_metricsManager.GetMetric(id) == null)
            {
                return NotFound(new
                {
                    message = "A metric with this ID does not exist."
                });
            }

            var metricEntries = _metricsManager.GetEntries(id, limit);
            return Ok(metricEntries);
        }

        [HttpDelete("{id}")]

        [SwaggerOperation(
            Summary = "Delete a metric by id.",
            Description = "Removes a metric and all associated entries."
        )]
        [SwaggerResponse(204)]
        public IActionResult DeleteMetric([FromRoute] int id)
        {
            _metricsManager.RemoveMetric(id);
            return NoContent();
        }

        [HttpPut("{id}/entries")]

        [SwaggerOperation(
            Summary = "Insert a new metric entry and associate it with a metric by id.",
            Description = "Creates a new metric entry and associates it with the provided metric id"
        )]
        [SwaggerResponse(200, "", typeof(MetricEntry))]
        [SwaggerResponse(404, "A metric with this ID does not exist.", typeof(Error))]
        public IActionResult SubmitEntry([FromRoute] int id, CreateMetricEntryRequest requestBody)
        {
            var metric = _metricsManager.GetMetric(id);
            if (metric == null)
            {
                return NotFound(new
                {
                    message = "A metric with this ID does not exist."
                });
            }

            var entry = requestBody.ToEntry();
            entry.MetricId = metric.Id;
            entry = _metricsManager.CreateEntry(entry);

            return Ok(entry);
        }

        [HttpDelete("{id}/entries")]

        [SwaggerOperation(
            Summary = "Delete all entries from a metric.",
            Description = "This endpoint is useful if you've been testing scripts that push to a custom metric, and you'd like to clear the sample entries before deploying."
        )]
        [SwaggerResponse(204)]
        [SwaggerResponse(404, "A metric with this ID does not exist.", typeof(Error))]
        public IActionResult DeleteEntries([FromRoute] int id)
        {
            var metric = _metricsManager.GetMetric(id);
            if (metric == null)
            {
                return NotFound(new
                {
                    message = "A metric with this ID does not exist."
                });
            }

            _metricsManager.ClearEntries(metric.Id);
            return NoContent();
        }
    }
}
