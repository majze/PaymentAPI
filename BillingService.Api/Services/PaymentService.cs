using BillingService.Api.Data;
using BillingService.Api.DTO;
using BillingService.Api.Models;

namespace BillingService.Api.Services;

public class PaymentService(BillingDbContext context) : IPaymentService
{
    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        var attempt = new PaymentAttempt
        {
            Id = Guid.NewGuid(),
            PolicyId = request.PolicyId,
            Amount = request.Amount,
            AttemptDate = DateTime.UtcNow,
            Success = request.shouldSucceed,
            RetryCount = 0
        };

        context.PaymentAttempts.Add(attempt);
        await context.SaveChangesAsync();

        return new PaymentResponse { Success = attempt.Success, AttemptId = attempt.Id };
    }

    public async Task<PaymentAttempt?> RetryPaymentAsync(Guid paymentId)
    {
        var payment = await context.PaymentAttempts.FindAsync(paymentId);
        if (payment == null) return null;

        payment.RetryCount += 1;
        await context.SaveChangesAsync();
        return payment;
    }
}
