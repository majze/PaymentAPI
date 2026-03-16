using BillingService.Api.Repositories;
using BillingService.Api.Models;
using BillingService.Api.DTO;

namespace BillingService.Api.Services;

public class AdminService(IPaymentPolicyRepository _repository, ILogger<AdminService> _logger) : IAdminService
{
    public async Task<List<PremiumSchedule>> GetAllPremiumSchedulesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all premium schedules");
            return await _repository.GetAllPremiumSchedulesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve all premium schedules");
            return new List<PremiumSchedule>();
        }
    }

    public async Task<List<DelinquentPolicyDto>> GetDelinquentPoliciesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all delinquent policies");
            return await _repository.GetDelinquentPoliciesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve delinquent policies");
            return new List<DelinquentPolicyDto>();
        }
    }
}
