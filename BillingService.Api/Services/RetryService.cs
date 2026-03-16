using BillingService.Api.Repositories;

namespace BillingService.Api.Services;

public class RetryService(IPaymentPolicyRepository _repository, ILogger<RetryService> _logger)
{
    public async Task RetryFailedPayments()
    {
        var failed = await _repository.GetFailedPaymentsAsync();

        foreach (var payment in failed)
        {
            _logger.LogInformation("Retrying payment {PaymentId}", payment.Id);

            bool success = new Random().Next(0, 2) == 1;

            payment.RetryCount++;

            if (success)
                payment.Success = true;
            else
                _logger.LogError("Failed retry of payment {PaymentId}", payment.Id);
        }

        await _repository.SaveChangesAsync();
    }
}