namespace BillingService.Api.Models;

public class Policy
{
    public Guid Id { get; set; }

    public string PolicyNumber { get; set; } = string.Empty;

    public bool IsDelinquent { get; set; } = false;

    public string CustomerName { get; set; } = string.Empty;

    public decimal Premium { get; set; } = default;
}