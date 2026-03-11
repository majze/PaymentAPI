using Microsoft.AspNetCore.Mvc;
using BillingService.Api.Services;
using BillingService.Api.DTO;

namespace BillingService.Api.Controllers;

[ApiController]
[Route("payments")]
public class PaymentsController(IPaymentService _paymentService, ILogger<PaymentsController> _logger) : ControllerBase
{
    [HttpPost("attempt")]
    public async Task<IActionResult> AttemptPayment([FromBody] PaymentRequest request)
    {
        // Use middleware to include correlation id in logs,
        // in reality this is more important when connecting multiple services,
        // and increased observability by allowing a full trace to identified.
        // The correlationId will be included in the log, but is fetched here as proof.
        var correlationId = HttpContext.Items["CorrelationId"] as string;

        _logger.LogInformation("Received payment attempt for policy {PolicyId} {CorrelationId}", 
            request.PolicyId, correlationId);

        // Embrace N-tier architecture where business logic is defined in services, 
        // while controllers are responsible for routing, logging, and transforming responses. 
        var result = await _paymentService.ProcessPaymentAsync(request);

        _logger.LogInformation("Payment recorded. AttemptId={AttemptId} {CorrelationId}",
            result.AttemptId, correlationId);

        return Ok(result);
    }

    [HttpPost("retry/{paymentId}")]
    public async Task<IActionResult> RetryPayment(Guid paymentId)
    {
        var payment = await _paymentService.RetryPaymentAsync(paymentId);
        return payment == null ? NotFound() : Ok(payment);
    }
}
