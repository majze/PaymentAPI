using Microsoft.EntityFrameworkCore;
using BillingService.Api.Models;

namespace BillingService.Api.Data;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Policy> Policies { get; set; }

    public DbSet<PaymentAttempt> PaymentAttempts { get; set; }

    public DbSet<PremiumSchedule> PremiumSchedules { get; set; }
}