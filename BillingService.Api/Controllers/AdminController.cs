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
        try
        {
        var result = await _adminService.GetAllPremiumSchedulesAsync();
        return Ok(result);
        }
        catch (Exception)
        {
            // Log the exception (not implemented here for brevity)
            return StatusCode(500, "An error occurred while fetching premium schedules.");
        }
    }

    [HttpGet("all-delinquent-policies")]
    public async Task<IActionResult> GetDelinquentPolicies()
    {
        try
        {
            var result = await _adminService.GetDelinquentPoliciesAsync();
            return Ok(result);
        }
        catch (Exception)
        {
            // Log the exception (not implemented here for brevity)
            return StatusCode(500, "An error occurred while fetching delinquent policies.");
        }
    }
}
