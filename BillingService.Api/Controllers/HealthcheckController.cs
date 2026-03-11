using Microsoft.AspNetCore.Mvc;

namespace BillingService.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthcheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Check()
    {
        return Ok("Healthy");
    }
}