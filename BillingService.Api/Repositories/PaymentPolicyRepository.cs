using Microsoft.EntityFrameworkCore;
using BillingService.Api.Data;
using BillingService.Api.DTO;
using BillingService.Api.Models;
using System.Diagnostics.CodeAnalysis;

namespace BillingService.Api.Repositories;

public interface IPaymentPolicyRepository
{
    // Payment-related methods
    Task<PaymentAttempt?> GetPaymentAttemptAsync(Guid attemptId);
    Task AddPaymentAttemptAsync(PaymentAttempt attempt);
    Task UpdatePaymentAttemptAsync(PaymentAttempt attempt);
    Task SaveChangesAsync();

    // Policies-related methods
    Task<PremiumSchedule?> GetPremiumScheduleAsync(Guid premiumScheduleId);

    // Admin-related methods
    Task<List<PremiumSchedule>> GetAllPremiumSchedulesAsync();
    Task<List<DelinquentPolicyDto>> GetDelinquentPoliciesAsync();

    // Retry-related methods
    Task<List<PaymentAttempt>> GetFailedPaymentsAsync();
}

[ExcludeFromCodeCoverage]
public class PaymentPolicyRepository : IPaymentPolicyRepository
{
    private readonly BillingDbContext _context;

    public PaymentPolicyRepository(BillingDbContext context)
    {
        _context = context;
    }

    // Payment-related implementations
    public async Task<PaymentAttempt?> GetPaymentAttemptAsync(Guid attemptId)
    {
        return await _context.PaymentAttempts.FindAsync(attemptId);
    }

    public async Task AddPaymentAttemptAsync(PaymentAttempt attempt)
    {
        await _context.PaymentAttempts.AddAsync(attempt);
    }

    public async Task UpdatePaymentAttemptAsync(PaymentAttempt attempt)
    {
        _context.PaymentAttempts.Update(attempt);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    // Policies-related implementations
    public async Task<PremiumSchedule?> GetPremiumScheduleAsync(Guid premiumScheduleId)
    {
        return await _context.PremiumSchedules.FindAsync(premiumScheduleId);
    }

    // Admin-related implementations
    public async Task<List<PremiumSchedule>> GetAllPremiumSchedulesAsync()
    {
        return await _context.PremiumSchedules.ToListAsync();
    }

    public async Task<List<DelinquentPolicyDto>> GetDelinquentPoliciesAsync()
    {
        return await _context.PremiumSchedules
            .Where(p => p.Status == "delinquent")
            .Select(p => new DelinquentPolicyDto(p.PolicyId, p.DueDate))
            .ToListAsync();
    }

    // Retry-related implementations
    public async Task<List<PaymentAttempt>> GetFailedPaymentsAsync()
    {
        return await _context.PaymentAttempts
            .Where(p => !p.Success && p.RetryCount < 3)
            .ToListAsync();
    }
}