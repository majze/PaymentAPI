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
        var correlationId = HttpContext.Items["CorrelationId"] as string;

        _logger.LogInformation("Received payment attempt for policy {PolicyId} {CorrelationId}", 
            request.PolicyId, correlationId);

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
