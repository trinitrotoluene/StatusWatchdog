using Microsoft.AspNetCore.Mvc;
using ServiceWatchdog.Api.Controllers.RequestModels;
using ServiceWatchdog.Api.Services;

namespace ServiceWatchdog.Api.Controllers
{
    [Route("api/v1/metrics")]
    [ApiController]
    public class MetricsController : Controller
    {
        private readonly MetricsManager _metricsManager;

        public MetricsController(MetricsManager metricsManager)
        {
            _metricsManager = metricsManager;
        }

        [HttpGet("{id}")]
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
        public IActionResult DeleteMetric([FromRoute] int id)
        {
            _metricsManager.RemoveMetric(id);
            return Ok();
        }

        [HttpPut("{id}/entries")]
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
            return Ok();
        }
    }
}
