using BillingService.Api.Models;
using BillingService.Api.DTO;

namespace BillingService.Api.Services;

public interface IAdminService
{
    Task<List<PremiumSchedule>> GetAllPremiumSchedulesAsync();
    Task<List<DelinquentPolicyDto>> GetDelinquentPoliciesAsync();
}
