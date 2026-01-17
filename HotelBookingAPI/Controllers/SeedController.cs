using Microsoft.AspNetCore.Mvc;
using HotelBookingAPI.Data;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly SeedService _seeder;
        private readonly IConfiguration _config;
        private readonly IHostEnvironment _env;

        public SeedController(SeedService seeder, IConfiguration config, IHostEnvironment env)
        {
            _seeder = seeder;
            _config = config;
            _env = env;
        }

        // POST api/seed?force=true
        [HttpPost]
        public async Task<IActionResult> Seed([FromQuery] bool force = false, CancellationToken cancellationToken = default)
        {
            if (!IsAuthorized()) return Forbid();

            await _seeder.SeedAsync(force, cancellationToken);

            return Ok(new { seeded = true, force });
        }

        // DELETE api/seed   (resets DB data)
        [HttpDelete]
        public async Task<IActionResult> Reset(CancellationToken cancellationToken = default)
        {
            if (!IsAuthorized()) return Forbid();

            await _seeder.ResetAsync(cancellationToken);

            return Ok(new { reset = true });
        }

        private bool IsAuthorized()
        {
            // Allow if running in Development and no key configured
            var key = _config["SeedApiKey"];
            if (string.IsNullOrEmpty(key) && _env.IsDevelopment()) return true;

            if (string.IsNullOrEmpty(key)) return false;

            if (!Request.Headers.TryGetValue("X-Seed-Key", out var provided)) return false;

            return string.Equals(provided.FirstOrDefault(), key, StringComparison.Ordinal);
        }
    }
}