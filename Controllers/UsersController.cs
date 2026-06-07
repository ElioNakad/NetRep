using Microsoft.AspNetCore.Mvc;

namespace MyAzureDemo.Controllers;

[ApiController]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(new[]
        {
            new { Id = 1, Name = "mtm" },
            new { Id = 2, Name = "John" }
        });
    }
}
