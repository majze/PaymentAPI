using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BillingService.Api.Data;
using BillingService.Api.DTO;

namespace BillingService.Api.Controllers;

[ApiController]
[Route("payments")]
public class PoliciesController : ControllerBase
{
    private readonly BillingDbContext _context;
    private readonly ILogger<PoliciesController> _logger;

    public PoliciesController(BillingDbContext context, ILogger<PoliciesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("{policyId}/premium-schedule")]
    public async Task<IActionResult> GetPremiumSchedule(Guid policyId)
    {
        var result = await _context.PremiumSchedules
            .Where(p => p.PolicyId == policyId)
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("delinquent")]
    public async Task<IActionResult> GetDelinquentPolicies()
    {
        var overduePolicies = await _context.PremiumSchedules
            .Where(p => p.Status == "overdue")
            .Select(p => new
            {
                p.PolicyId,
                p.DueDate
            })
            .ToListAsync();

        return Ok(overduePolicies);
    }
}