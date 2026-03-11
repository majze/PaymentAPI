using Microsoft.AspNetCore.Mvc;
using BillingService.Api.Data;
using BillingService.Api.Services;
using BillingService.Api.DTO;

namespace BillingService.Api.Controllers;

public class RetryController : ControllerBase
{
    private readonly BillingDbContext _context;
    private readonly ILogger<RetryController> _logger;
    private readonly RetryService _retryService;

    public RetryController(BillingDbContext context, ILogger<RetryController> logger, RetryService retryService)
    {
        _context = context;
        _logger = logger;
        _retryService = retryService;
    }

    // Migrating this to a worker
    // [HttpPost("retry/{policyId}")]
    // public async Task<IActionResult> RetryPayment(Guid policyId)
    // {
    //     await _retryService.RetryFailedPayments(policyId);

    //     return Ok(new
    //     {
    //         status = "retry triggered"
    //     });
    // }
}
