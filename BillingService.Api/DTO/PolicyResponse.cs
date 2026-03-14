using BillingService.Api.Models;

namespace BillingService.Api.DTO;

public class PolicyResponse
{
    public bool Success { get; set; } = false;

    public PremiumSchedule? PremiumSchedule { get; set; }

    public string? Message { get; set; }
}