using Microsoft.AspNetCore.Mvc;
using BillingService.Api.Services;

namespace BillingService.Api.Controllers;

[ApiController]
[Route("admin")]
public class AdminController(IAdminService _adminService) : ControllerBase
{
    [HttpGet("all-premium-schedules")]
    public async Task<IActionResult> GetAllPremiumSchedules()
    {
        var result = await _adminService.GetAllPremiumSchedulesAsync();
        return Ok(result);
    }

    [HttpGet("all-delinquent-policies")]
    public async Task<IActionResult> GetDelinquentPolicies()
    {
        var result = await _adminService.GetDelinquentPoliciesAsync();
        return Ok(result);
    }
}
