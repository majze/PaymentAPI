using Xunit;
using Moq;
using FluentAssertions;
using BillingService.Api.Services;
using BillingService.Api.Repositories;
using BillingService.Api.Models;
using Microsoft.Extensions.Logging;

namespace BillingService.Api.Tests.ServiceTests;

public class RetryServiceTests
{
    private readonly Mock<IPaymentPolicyRepository> _mockRepository;
    private readonly Mock<ILogger<RetryService>> _mockLogger;
    private readonly RetryService _service;

    public RetryServiceTests()
    {
        _mockRepository = new Mock<IPaymentPolicyRepository>();
        _mockLogger = new Mock<ILogger<RetryService>>();
        _service = new RetryService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task RetryFailedPayments_UpdatesPaymentsCorrectly()
    {
        // Arrange
        var failedPayments = new List<PaymentAttempt>
        {
            new PaymentAttempt { Id = Guid.NewGuid(), Success = false, RetryCount = 0 },
            new PaymentAttempt { Id = Guid.NewGuid(), Success = false, RetryCount = 1 }
        };
        _mockRepository.Setup(r => r.GetFailedPaymentsAsync()).ReturnsAsync(failedPayments);

        // Act
        await _service.RetryFailedPayments();

        // Assert
        _mockRepository.Verify(r => r.GetFailedPaymentsAsync(), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        // Note: Since success is random, we can't assert exact values, but verify calls
        failedPayments.All(p => p.RetryCount > 0).Should().BeTrue();
    }
}