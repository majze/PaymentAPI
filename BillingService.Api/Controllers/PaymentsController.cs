using Microsoft.AspNetCore.Mvc;
using BillingService.Api.Data;
using BillingService.Api.Models;
using BillingService.Api.DTO;

namespace BillingService.Api.Controllers;

[ApiController]
[Route("payments")]
public class PaymentsController(BillingDbContext _context, ILogger<PaymentsController> _logger) : ControllerBase
{
    [HttpPost("attempt")]
    public async Task<IActionResult> AttemptPayment([FromBody] PaymentRequest request)
    {
        // Use middleware to include correlation id in logs,
        // in reality this is more important when connecting multiple services,
        // and increased observability by allowing a full trace to identified.
        // The correlationId will be included in the log, but is fetched here as proof.
        var correlationId = HttpContext.Items["CorrelationId"] as string;

        _logger.LogInformation("Received payment attempt for policy {PolicyId} with requested amount {Amount} {CorrelationId}", 
            request.PolicyId,
            request.Amount,
            correlationId);

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
            "Payment attempt recorded. PolicyId={PolicyId} Result={Success} AttemptId={AttemptId} {CorrelationId}",
            request.PolicyId,
            attempt.Success,
            attempt.Id,
            correlationId
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