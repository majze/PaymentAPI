namespace BillingService.Api.Models;

public class PaymentAttempt
{
    public Guid Id { get; set; }

    public Guid PolicyId { get; set; }

    public decimal Amount { get; set; }

    public DateTime AttemptDate { get; set; }

    public bool Success { get; set; }

    public int RetryCount { get; set; }
}