using BillingService.Api.Data;
using BillingService.Api.DTO;

namespace BillingService.Api.Services;

public class PoliciesService(BillingDbContext _context, ILogger<PoliciesService> _logger)
{
    public async Task<PolicyResponse> GetPremiumSchedule(Guid premiumScheduleId)
    {
        try
        {
            _logger.LogInformation("Fetching premium schedule {premiumScheduleId}", premiumScheduleId);

            var premiumSchedule = await _context.PremiumSchedules.FindAsync(premiumScheduleId);

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
