namespace BillingService.Api.DTO;

public class PaymentResponse
{
    public bool Success { get; set; } = false;

    public Guid AttemptId { get; set; } 
}