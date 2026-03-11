using BillingService.Api.DTO;
using BillingService.Api.Models;

namespace BillingService.Api.Services;

public interface IPaymentService
{
    Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    Task<PaymentAttempt?> RetryPaymentAsync(Guid paymentId);
}
