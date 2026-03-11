namespace BillingService.Api.Models;

public class Policy
{
    public Guid Id { get; set; }

    public string PolicyNumber { get; set; }
    
    public bool IsDelinquent { get; set; }
}