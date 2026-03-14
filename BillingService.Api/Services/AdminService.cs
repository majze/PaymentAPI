using BillingService.Api.Data;
using Microsoft.EntityFrameworkCore;
using BillingService.Api.Models;

namespace BillingService.Api.Services;

public class AdminService(BillingDbContext _context, ILogger<AdminService> _logger) : IAdminService
{
    public async Task<List<PremiumSchedule>> GetAllPremiumSchedulesAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all premium schedules");
            return await _context.PremiumSchedules.ToListAsync();
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
            
            return await _context.PremiumSchedules
                .Where(p => p.Status == "delinquent")
                .Select(p => new DelinquentPolicyDto(p.PolicyId, p.DueDate))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve delinquent policies");
            return new List<DelinquentPolicyDto>();
        }
    }
}
