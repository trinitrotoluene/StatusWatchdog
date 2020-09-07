using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatusWatchdog.Controllers.RequestModels;
using StatusWatchdog.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace StatusWatchdog.Controllers
{
    [Authorize]
    [Route("api/v1/kv")]
    [ApiController]
    public class KeyValuesController : Controller
    {
        private readonly KeyValuesManager _kvManager;

        public KeyValuesController(KeyValuesManager kvManager)
        {
            _kvManager = kvManager;
        }

        [HttpPut]

        [SwaggerOperation(
            Summary = "Create or update an entry in KV storage.",
            Description = "This endpoint creates or updates a new key-value association in the KV table."
        )]
        [SwaggerResponse(204)]
        public IActionResult SetKV([FromBody] CreateKeyValueRequest requestBody)
        {
            _kvManager.SetValue(requestBody.Key, requestBody.Value);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost]

        [SwaggerOperation(
            Summary = "Bulk retrieve values from KV storage.",
            Description = "The KV API can be provided with an array of strings to search the values of. Any missing entries will be returned with a null value."
        )]
        [SwaggerResponse(200, "", typeof(IDictionary<string, string>))]
        public IActionResult RequestKVs([FromBody] string[] keys)
        {
            var kvs = _kvManager.GetValues(keys);
            return Ok(kvs);
        }

        [HttpDelete]

        [SwaggerOperation(
            Summary = "Delete a key-value association from the KV table."
        )]
        [SwaggerResponse(204)]
        public IActionResult DeleteKV([FromQuery] string key)
        {
            _kvManager.DeleteKey(key);
            return NoContent();
        }
    }
}
