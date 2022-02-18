using BackendSession2.Core.Models;
using BackendSession2.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BackendSession2.Controllers
{
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        public CacheController(ICacheService i)
        {
            _cacheService = i;
        }

        [HttpGet("cache/{key}")]
        public async Task<IActionResult> GetCacheValue([FromRoute] string key)
        {
            var value = await _cacheService.GetCacheValueAsync(key);
            return string.IsNullOrEmpty(value) ? (IActionResult) NotFound() : Ok(value);
        }

        [HttpPost("cache")]
        public async Task<IActionResult> SetCacheValue([FromBody] NewCacheEntryRequest request)
        {
            await _cacheService.SetCacheValueAsync(request.key, request.value);
            return Ok();
        }
    }
}
