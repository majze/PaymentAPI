namespace BillingService.Api.Models;

public class PremiumSchedule
{
    public Guid Id { get; set; }

    public Guid PolicyId { get; set; }

    public string PolicyNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public decimal CoveredAmount { get; set; } = default;

    public decimal AmountDue { get; set; } = default;

    public DateTime DueDate { get; set; }
}