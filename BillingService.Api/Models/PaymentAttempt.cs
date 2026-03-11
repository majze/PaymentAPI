namespace BillingService.Api.Models;

public class PaymentAttempt
{
    public Guid Id { get; set; }

    public Guid PolicyId { get; set; }

    public decimal Amount { get; set; } = default;

    public DateTime AttemptDate { get; set; } = DateTime.Now;

    public bool Success { get; set; } = false;

    public int RetryCount { get; set; } = 0;
}