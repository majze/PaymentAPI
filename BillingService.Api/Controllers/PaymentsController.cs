using Microsoft.AspNetCore.Mvc;
using BillingService.Api.Data;
using BillingService.Api.Models;
using BillingService.Api.DTO;

namespace BillingService.Api.Controllers;

[ApiController]
[Route("payments")]
public class PaymentsController : ControllerBase
{
    private readonly BillingDbContext _context;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(BillingDbContext context, ILogger<PaymentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpPost("attempt")]
    public async Task<IActionResult> AttemptPayment([FromBody] PaymentRequest request)
    {
        _logger.LogInformation("Received payment attempt for policy {PolicyId} with requested amount {Amount}", 
            request.PolicyId,
            request.Amount);

        

        var attempt = new PaymentAttempt
        {
            Id = Guid.NewGuid(),
            PolicyId = request.PolicyId,
            Amount = request.Amount,
            AttemptDate = DateTime.UtcNow,
            Success = request.shouldSucceed,
            RetryCount = 0
        };

        _context.PaymentAttempts.Add(attempt);
        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Payment attempt recorded. PolicyId={PolicyId} Result={Success} AttemptId={AttemptId}",
            request.PolicyId,
            attempt.Success,
            attempt.Id
        );

        return Ok(new
        {
            result = attempt.Success,
            attemptId = attempt.Id
        });
    }

    [HttpPost("retry/{paymentId}")]
    public async Task<IActionResult> RetryPayment(Guid paymentId)
    {
        var payment = await _context.PaymentAttempts.FindAsync(paymentId);

        if (payment == null)
            return NotFound();

        payment.RetryCount += 1;

        await _context.SaveChangesAsync();

        return Ok(payment);
    }
}