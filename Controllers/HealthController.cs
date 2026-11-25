using Microsoft.AspNetCore.Mvc;
using QualiTrack.Helpers;

namespace QualiTrack.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var timestamp = DateTime.UtcNow;

        return Ok(new
        {
            status = "ok",
            timestamp,
            timestampFormatted = DateHelper.FormatDateTime(timestamp),
            environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            version = "1.0.0"
        });
    }
}