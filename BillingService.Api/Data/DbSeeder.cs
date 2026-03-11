using BillingService.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BillingService.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(BillingDbContext context)
    {
        // Reset Data - This will clear the data in case you need to modify the seed script several times like I did.
        // Comment the two lines out if we want data to persist.
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"PremiumSchedules\" RESTART IDENTITY CASCADE;");
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"Policies\" RESTART IDENTITY CASCADE;");

        if (context.Policies.Any())
            return;

        var policy1 = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerName = "Alice Johnson",
            Premium = 120
        };

        var policy2 = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerName = "Bob Smith",
            Premium = 95
        };

        var policy3 = new Policy
        {
            Id = Guid.NewGuid(),
            CustomerName = "Charlie Brown",
            Premium = 150
        };

        context.Policies.AddRange(policy1, policy2, policy3);

        context.PremiumSchedules.AddRange(
            new PremiumSchedule
            {
                PolicyId = policy1.Id,
                PolicyNumber = "P001",
                Status = "delinquent",
                DueDate = DateTime.UtcNow.AddDays(30),
                Amount = 120
            },
            new PremiumSchedule
            {
                PolicyId = policy1.Id,
                PolicyNumber = "P001",
                Status = "delinquent",
                DueDate = DateTime.UtcNow.AddDays(60),
                Amount = 120
            },
            new PremiumSchedule
            {
                PolicyId = policy2.Id,
                PolicyNumber = "P002",
                Status = "Payment Due",
                DueDate = DateTime.UtcNow.AddDays(30),
                Amount = 95
            },
            new PremiumSchedule
            {
                PolicyId = policy2.Id,
                PolicyNumber = "P002",
                Status = "Payment Due",
                DueDate = DateTime.UtcNow.AddDays(60),
                Amount = 95
            },
            new PremiumSchedule
            {
                PolicyId = policy3.Id,
                PolicyNumber = "P003",
                Status = "Payment Due",
                DueDate = DateTime.UtcNow.AddDays(30),
                Amount = 150
            },
            new PremiumSchedule
            {
                PolicyId = policy3.Id,
                PolicyNumber = "P003",
                Status = "Payment Due",
                DueDate = DateTime.UtcNow.AddDays(60),
                Amount = 150
            }
        );

        await context.SaveChangesAsync();
    }
}