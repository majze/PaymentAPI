using Xunit;
using Moq;
using FluentAssertions;
using BillingService.Api.Services;
using BillingService.Api.Repositories;
using BillingService.Api.Models;
using BillingService.Api.DTO;
using Microsoft.Extensions.Logging;

namespace BillingService.Api.Tests.ServiceTests;

public class AdminServiceTest
{
    private readonly Mock<IPaymentPolicyRepository> _mockRepository;
    private readonly Mock<ILogger<AdminService>> _mockLogger;
    private readonly AdminService _service;

    public AdminServiceTest()
    {
        _mockRepository = new Mock<IPaymentPolicyRepository>();
        _mockLogger = new Mock<ILogger<AdminService>>();
        _service = new AdminService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllPremiumSchedulesAsync_ReturnsList_WhenSuccessful()
    {
        // Arrange
        var schedules = new List<PremiumSchedule>
        {
            new PremiumSchedule { Id = Guid.NewGuid(), Amount = 100.0m },
            new PremiumSchedule { Id = Guid.NewGuid(), Amount = 200.0m }
        };
        _mockRepository.Setup(r => r.GetAllPremiumSchedulesAsync()).ReturnsAsync(schedules);

        // Act
        var result = await _service.GetAllPremiumSchedulesAsync();

        // Assert
        result.Should().BeEquivalentTo(schedules);
        _mockRepository.Verify(r => r.GetAllPremiumSchedulesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllPremiumSchedulesAsync_ReturnsEmptyList_WhenExceptionThrown()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllPremiumSchedulesAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.GetAllPremiumSchedulesAsync();

        // Assert
        result.Should().BeEmpty();
        _mockRepository.Verify(r => r.GetAllPremiumSchedulesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetDelinquentPoliciesAsync_ReturnsList_WhenSuccessful()
    {
        // Arrange
        var policies = new List<DelinquentPolicyDto>
        {
            new DelinquentPolicyDto(Guid.NewGuid(), DateTime.UtcNow),
            new DelinquentPolicyDto(Guid.NewGuid(), DateTime.UtcNow.AddDays(-1))
        };
        _mockRepository.Setup(r => r.GetDelinquentPoliciesAsync()).ReturnsAsync(policies);

        // Act
        var result = await _service.GetDelinquentPoliciesAsync();

        // Assert
        result.Should().BeEquivalentTo(policies);
        _mockRepository.Verify(r => r.GetDelinquentPoliciesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetDelinquentPoliciesAsync_ReturnsEmptyList_WhenExceptionThrown()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetDelinquentPoliciesAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.GetDelinquentPoliciesAsync();

        // Assert
        result.Should().BeEmpty();
        _mockRepository.Verify(r => r.GetDelinquentPoliciesAsync(), Times.Once);
    }
}