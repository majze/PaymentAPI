using BillingService.Api.DTO;
using BillingService.Api.Models;
using BillingService.Api.Repositories;

namespace BillingService.Api.Services;

public class PaymentService(IPaymentPolicyRepository _repository, ILogger<PaymentService> _logger) : IPaymentService
{
    public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            var attempt = new PaymentAttempt
            {
                Id = Guid.NewGuid(),
                PolicyId = request.PolicyId,
                Amount = request.Amount,
                AttemptDate = DateTime.UtcNow,
                Success = request.ShouldSucceed,
                RetryCount = 0
            };

            _logger.LogInformation("Saving payment attempt for {PolicyId} with amount {Amount} {Id}",
                request.PolicyId, 
                request.Amount, 
                attempt.Id);

            await _repository.AddPaymentAttemptAsync(attempt);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Payment attempt successful {Id}", attempt.Id);

            return new PaymentResponse { Success = attempt.Success, AttemptId = attempt.Id };
        }
        catch (Exception ex)
        {
            _logger.LogError("Saving of payment attempt failed {PolicyId} {exception}", request, ex.Message);

            return new PaymentResponse { Success = false, AttemptId = request.PolicyId };
        }
    }

    public async Task<PaymentAttempt?> RetryPaymentAsync(Guid paymentId)
    {
        try
        {
            _logger.LogInformation("Retry payment attempt {Id}", paymentId);

            var payment = await _repository.GetPaymentAttemptAsync(paymentId);
            if (payment == null) return null;

            payment.RetryCount += 1;
            await _repository.UpdatePaymentAttemptAsync(payment);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Retry payment attempt successful {Id}", paymentId);

            return payment;
        }
        catch (Exception ex)
        {
            _logger.LogError("Retry of payment attempt {Id} failed {exception}", paymentId, ex.Message);
            
            return new PaymentAttempt() { Id = paymentId, Success = false };
        }
    }
}
