namespace BillingService.Api.DTO;

public class PaymentRequest
{
    public Guid PolicyId { get; set; }

    public decimal Amount { get; set; }

    // Manual way of triggering a failure
    public bool shouldSucceed { get; set; } 
}