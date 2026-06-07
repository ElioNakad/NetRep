using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAzureDemo.Data;

namespace MyAzureDemo.Controllers;

[ApiController]
[Route("")]
public sealed class HealthController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet("")]
    public IActionResult Home()
    {
        return Ok("Hello akhi");
    }

    [HttpGet("db-health")]
    public async Task<IActionResult> DatabaseHealth()
    {
        var canConnect = await db.Database.CanConnectAsync();

        return canConnect
            ? Ok(new { Status = "Connected to PostgreSQL" })
            : Problem("Could not connect to PostgreSQL");
    }
}
