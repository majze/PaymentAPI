using BillingService.Api.Models;

namespace BillingService.Api.Services;

public interface IAdminService
{
    Task<List<PremiumSchedule>> GetAllPremiumSchedulesAsync();
    Task<List<DelinquentPolicyDto>> GetDelinquentPoliciesAsync();
}

public record DelinquentPolicyDto(Guid PolicyId, DateTime DueDate);
