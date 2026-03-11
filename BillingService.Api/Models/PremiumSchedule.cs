namespace BillingService.Api.Models;

public class PremiumSchedule
{
    public Guid Id { get; set; }

    public Guid PolicyId { get; set; }

    public string Status { get; set; }

    public decimal AmountDue { get; set; }

    public DateTime DueDate { get; set; }
}