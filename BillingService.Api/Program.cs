using Microsoft.EntityFrameworkCore;
using BillingService.Api.Data;
using BillingService.Api.Workers;
using BillingService.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("BillingDb"));

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

app.MapControllers();
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
app.UseMiddleware<CorrelationIdMiddleware>();

// redirect root to swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();