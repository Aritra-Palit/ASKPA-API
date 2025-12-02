using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace ASKPA_API.Controllers
{
    [ApiController]
    [Route("askpa/admin")]
    public class TempStoreController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(10);

        public TempStoreController(IMemoryCache cache)
        {
            _cache = cache;
        }

        public class SaveRequest
        {
            public object Data { get; set; }
            public int? ExpiresInSeconds { get; set; }
            public bool OneTimeFetch { get; set; } = true;
        }

        public class SaveResponse
        {
            public string Token { get; set; }
            public DateTime ExpiresAt { get; set; }
        }

        [HttpPost("save")]
        public IActionResult Save([FromBody] SaveRequest req)
        {
            if (req?.Data == null)
                return BadRequest("No data provided");

            var token = Guid.NewGuid().ToString("N");
            var ttl = TimeSpan.FromSeconds(req.ExpiresInSeconds ?? (int)DefaultTtl.TotalSeconds);

            var container = new
            {
                payload = req.Data,
                oneTime = req.OneTimeFetch
            };

            _cache.Set(token, container, ttl);

            return Ok(new SaveResponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.Add(ttl)
            });
        }

        [HttpGet("fetch/{token}")]
        public IActionResult Fetch(string token)
        {
            if (!_cache.TryGetValue(token, out object stored))
                return NotFound();

            var storedJson = System.Text.Json.JsonSerializer.Serialize(stored);
            using var doc = System.Text.Json.JsonDocument.Parse(storedJson);
            var root = doc.RootElement;

            var payloadJson = root.GetProperty("payload").GetRawText();
            bool oneTime = root.GetProperty("oneTime").GetBoolean();

            if (oneTime)
                _cache.Remove(token);

            return Content(payloadJson, "application/json");
        }
    }
}
