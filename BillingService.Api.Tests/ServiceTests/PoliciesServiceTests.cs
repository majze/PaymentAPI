using Xunit;
using Moq;
using FluentAssertions;
using BillingService.Api.Services;
using BillingService.Api.Repositories;
using BillingService.Api.DTO;
using BillingService.Api.Models;
using Microsoft.Extensions.Logging;

namespace BillingService.Api.Tests.ServiceTests;

public class PoliciesServiceTests
{
    private readonly Mock<IPaymentPolicyRepository> _mockRepository;
    private readonly Mock<ILogger<PoliciesService>> _mockLogger;
    private readonly PoliciesService _service;

    public PoliciesServiceTests()
    {
        _mockRepository = new Mock<IPaymentPolicyRepository>();
        _mockLogger = new Mock<ILogger<PoliciesService>>();
        _service = new PoliciesService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetPremiumSchedule_ReturnsSuccessResponse_WhenPremiumScheduleFound()
    {
        // Arrange
        var premiumScheduleId = Guid.NewGuid();
        var premiumSchedule = new PremiumSchedule { Id = premiumScheduleId, Amount = 150.0m };
        _mockRepository.Setup(r => r.GetPremiumScheduleAsync(premiumScheduleId)).ReturnsAsync(premiumSchedule);

        // Act
        var result = await _service.GetPremiumSchedule(premiumScheduleId);

        // Assert
        result.Success.Should().BeTrue();
        result.PremiumSchedule.Should().Be(premiumSchedule);
        result.Message.Should().BeNull();
        _mockRepository.Verify(r => r.GetPremiumScheduleAsync(premiumScheduleId), Times.Once);
    }

    [Fact]
    public async Task GetPremiumSchedule_ReturnsFailureResponse_WhenExceptionThrown()
    {
        // Arrange
        var premiumScheduleId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetPremiumScheduleAsync(premiumScheduleId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.GetPremiumSchedule(premiumScheduleId);

        // Assert
        result.Success.Should().BeFalse();
        result.PremiumSchedule.Should().BeNull();
        result.Message.Should().Be("Something went wrong, please contact support if this issue persists");
        _mockRepository.Verify(r => r.GetPremiumScheduleAsync(premiumScheduleId), Times.Once);
    }
}