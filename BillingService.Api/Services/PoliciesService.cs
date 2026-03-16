using BillingService.Api.DTO;
using BillingService.Api.Repositories;

namespace BillingService.Api.Services;

public class PoliciesService(IPaymentPolicyRepository _repository, ILogger<PoliciesService> _logger) : IPoliciesService
{
    public async Task<PolicyResponse> GetPremiumSchedule(Guid premiumScheduleId)
    {
        try
        {
            _logger.LogInformation("Fetching premium schedule {premiumScheduleId}", premiumScheduleId);

            var premiumSchedule = await _repository.GetPremiumScheduleAsync(premiumScheduleId);

            return new PolicyResponse()
            {
                Success = true,
                PremiumSchedule = premiumSchedule    
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve premium schedule {premiumScheduleId}", premiumScheduleId);

            return new PolicyResponse() { Success = false, Message = "Something went wrong, please contact support if this issue persists"};
        }
    }
}
