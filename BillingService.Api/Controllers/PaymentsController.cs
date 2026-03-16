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
        if (payment != null)
        {
            _logger.LogInformation("Payment retry successful. AttemptId={AttemptId}", payment.Id);
            var response = new PaymentResponse { AttemptId = payment.Id, Success = true };
            return Ok(response);
        }
        else
        {
            _logger.LogWarning("Payment retry failed. PaymentId={PaymentId} not found.", paymentId);
            var response = new PaymentResponse { AttemptId = paymentId, Success = false };
            return NotFound(response);
        }
    }
}
