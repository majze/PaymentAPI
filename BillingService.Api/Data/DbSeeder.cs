using BillingService.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(BillingDbContext context, bool shouldPersistData)
    {
        if (!shouldPersistData)
        {
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PremiumSchedules\" RESTART IDENTITY CASCADE;");
            await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Policies\" RESTART IDENTITY CASCADE;");
        }

        if (context.Policies.Any())
            return;

        List<Policy> _polices = [
            AddNewPolicy(Guid.NewGuid(), "Alice Lui", 1.2E6m),
            AddNewPolicy(Guid.NewGuid(), "Austin Powers", 800E5m),
            AddNewPolicy(Guid.NewGuid(), "Charlie Brown", 1E6m)
        ];

        context.Policies.AddRange(_polices);

        await context.SaveChangesAsync();

        var _schedules = _polices.SelectMany((p, index) => new[]
        {
            AddNewSchedule(p.Id, $"P00{index + 1}", index == 0 ? "delinquent" : "Payment Due", 30, GetAmount(index))
        }).ToList();

        context.PremiumSchedules.AddRange(_schedules);

        await context.SaveChangesAsync();
    }

    private static decimal GetAmount(int index) => index switch { 0 => 200, 1 => 200, 2 => 20, _ => 30 };

    private static Policy AddNewPolicy(Guid id, string customerName, decimal premium)
    {
        return new Policy() {
            Id = id,
            CustomerName = customerName,
            Premium = premium
        };
    }

    private static PremiumSchedule AddNewSchedule(Guid policyId, string policyNumber, string status, int daysAhead, decimal amount)
    {
        return new PremiumSchedule
        {
            PolicyId = policyId,
            PolicyNumber = policyNumber,
            Status = status,
            DueDate = DateTime.UtcNow.AddDays(daysAhead),
            Amount = amount
        };
    }
}