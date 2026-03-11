using BillingService.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(BillingDbContext context)
    {
        // Debugging development - This will clear the data in case you need to modify the seed script several times like I did.
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PremiumSchedules\" RESTART IDENTITY CASCADE;");
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Policies\" RESTART IDENTITY CASCADE;");

        if (await context.Policies.AnyAsync())
            return; // already seeded

        var policies = new List<Policy>
        {
            new Policy
            {
                PolicyNumber = "POL1001",
                CustomerName = "Alice Johnson",
                Premium = 120.50m
            },
            new Policy
            {
                PolicyNumber = "POL1002",
                CustomerName = "Bob Smith",
                Premium = 89.99m
            },
            new Policy
            {
                PolicyNumber = "POL1003",
                CustomerName = "Charlie Brown",
                Premium = 150.00m
            }
        };

        context.Policies.AddRange(policies);

        var schedules = new List<PremiumSchedule>
        {
            new PremiumSchedule { PolicyNumber="POL1001", PolicyId=policies[0].Id, Status="Paid", DueDate=DateTime.UtcNow.AddDays(30), CoveredAmount=120.50m },
            new PremiumSchedule { PolicyNumber="POL1001", PolicyId=policies[0].Id, Status="Paid", DueDate=DateTime.UtcNow.AddDays(60), CoveredAmount=120.50m },

            new PremiumSchedule { PolicyNumber="POL1002", PolicyId=policies[1].Id, Status="Payment Due", AmountDue=30, DueDate=DateTime.UtcNow.AddDays(30), CoveredAmount=89.99m },
            new PremiumSchedule { PolicyNumber="POL1002", PolicyId=policies[1].Id, Status="Payment Due", AmountDue=30, DueDate=DateTime.UtcNow.AddDays(60), CoveredAmount=89.99m },

            new PremiumSchedule { PolicyNumber="POL1003", PolicyId=policies[2].Id, Status="Delinquent", AmountDue=120, DueDate=DateTime.UtcNow.AddDays(30), CoveredAmount=150.00m },
            new PremiumSchedule { PolicyNumber="POL1003", PolicyId=policies[2].Id, Status="Delinquent", AmountDue=120, DueDate=DateTime.UtcNow.AddDays(60), CoveredAmount=150.00m }
        };

        context.PremiumSchedules.AddRange(schedules);

        await context.SaveChangesAsync();
    }
}