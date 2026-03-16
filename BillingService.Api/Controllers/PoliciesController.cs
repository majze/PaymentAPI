using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BillingService.Api.Data;
using BillingService.Api.DTO;
using BillingService.Api.Services;

namespace BillingService.Api.Controllers;

[ApiController]
[Route("payments")]
public class PoliciesController(IPoliciesService _policiesService) : ControllerBase
{
    [HttpGet("{policyId}/premium-schedule")]
    public async Task<IActionResult> GetPremiumSchedule(Guid premiumScheduleId)
    {
        PolicyResponse response = await _policiesService.GetPremiumSchedule(premiumScheduleId);
        return Ok(response);
    }
}