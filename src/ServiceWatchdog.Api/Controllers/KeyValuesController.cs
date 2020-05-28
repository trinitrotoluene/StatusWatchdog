using Microsoft.AspNetCore.Mvc;
using ServiceWatchdog.Api.Controllers.RequestModels;
using ServiceWatchdog.Api.Services;

namespace ServiceWatchdog.Api.Controllers
{
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
        public IActionResult SetKV([FromBody] CreateKeyValueRequest requestBody)
        {
            _kvManager.SetValue(requestBody.Key, requestBody.Value);
            return Ok();
        }

        [HttpPost]
        public IActionResult RequestKVs([FromBody] string[] keys)
        {
            var kvs = _kvManager.GetValues(keys);
            return Ok(kvs);
        }

        [HttpDelete]
        public IActionResult DeleteKV([FromQuery] string key)
        {
            _kvManager.DeleteKey(key);
            return Ok();
        }
    }
}