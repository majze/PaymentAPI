using Microsoft.EntityFrameworkCore;
using BillingService.Api.Data;

namespace BillingService.Api.Services;

public class RetryService(BillingDbContext _context, ILogger<RetryService> _logger)
{
    public async Task RetryFailedPayments()
    {
        var failed = await _context.PaymentAttempts
            .Where(p => !p.Success &&
                        p.RetryCount < 3)
            .ToListAsync();

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

        await _context.SaveChangesAsync();
    }
}