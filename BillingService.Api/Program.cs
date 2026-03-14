using Microsoft.EntityFrameworkCore;
using BillingService.Api.Data;
using BillingService.Api.Workers;
using BillingService.Api.Middleware;
using BillingService.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("BillingDb"));
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<PoliciesService, PoliciesService>();
builder.Services.AddScoped<RetryService>();

// Database Connection
builder.Services.AddDbContext<BillingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BillingDb")));

// Register worker
builder.Services.AddHostedService<PaymentRetryWorker>();

// Build Application
var app = builder.Build();

// Enabling swagger in production for demonstration only
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<CorrelationIdMiddleware>();
app.MapControllers();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");

// redirect root to swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

// Create schema on startup and reset data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BillingDbContext>();
    
    bool shouldPersistData = false;

    // Retry logic to wait for Postgres to wake up
    for (int i = 0; i < 10; i++) 
    {
        try 
        {
            if (!shouldPersistData) {
                await db.Database.EnsureDeletedAsync();
                await db.Database.MigrateAsync();
            }

            await DbSeeder.SeedAsync(db, shouldPersistData);
            break;
        }
        catch (Npgsql.NpgsqlException)
        {
            if (i == 9) throw; // Fail after 10 tries
            Console.WriteLine("Postgres not ready... waiting 2 seconds...");
            await Task.Delay(2000);
        }
    }
}

app.Run();