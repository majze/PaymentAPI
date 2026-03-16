using BillingService.Api.DTO;
using BillingService.Api.Models;

namespace BillingService.Api.Services;

public interface IPoliciesService
{
    Task<PolicyResponse> GetPremiumSchedule(Guid scheduleId);
}
