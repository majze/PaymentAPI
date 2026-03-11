using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BillingService.Api.Services;

namespace BillingService.Api.Workers;

public class PaymentRetryWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PaymentRetryWorker> _logger;

    public PaymentRetryWorker(
        IServiceProvider serviceProvider,
        ILogger<PaymentRetryWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment retry worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRetries();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during retry processing");
            }

            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    private async Task ProcessRetries()
    {
        using var scope = _serviceProvider.CreateScope();

        var retryService = scope.ServiceProvider
            .GetRequiredService<RetryService>();

        _logger.LogInformation("Checking for failed payments");

        await retryService.RetryFailedPayments();
    }
}