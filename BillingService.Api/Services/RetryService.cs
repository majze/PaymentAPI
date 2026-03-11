using Microsoft.EntityFrameworkCore;
using BillingService.Api.Data;
using BillingService.Api.DTO;

namespace BillingService.Api.Services;

public class RetryService
{
    private readonly BillingDbContext _context;
    private readonly ILogger<RetryService> _logger;

    public RetryService(
        BillingDbContext context,
        ILogger<RetryService> logger)
    {
        _context = context;
        _logger = logger;
    }

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
        }

        await _context.SaveChangesAsync();
    }
}