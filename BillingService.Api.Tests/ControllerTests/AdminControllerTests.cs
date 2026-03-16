using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using BillingService.Api.Controllers;
using BillingService.Api.Services;
using BillingService.Api.Models;
using BillingService.Api.DTO;

namespace BillingService.Api.Tests.Controllers;

public class AdminControllerTests
{
    private readonly Mock<IAdminService> _mockAdminService;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        _mockAdminService = new Mock<IAdminService>();
        _controller = new AdminController(_mockAdminService.Object);
    }

    [Fact]
    public async Task GetAllPremiumSchedules_ReturnsOkWithSchedules()
    {
        // Arrange
        var schedules = new List<PremiumSchedule>
        {
            new PremiumSchedule { Id = Guid.NewGuid(), Amount = 100.0m },
            new PremiumSchedule { Id = Guid.NewGuid(), Amount = 200.0m }
        };
        _mockAdminService.Setup(s => s.GetAllPremiumSchedulesAsync()).ReturnsAsync(schedules);

        // Act
        var result = await _controller.GetAllPremiumSchedules();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(schedules);
    }

    [Fact]
    public async Task GetDelinquentPolicies_ReturnsOkWithPolicies()
    {
        // Arrange
        var policies = new List<DelinquentPolicyDto>
        {
            new DelinquentPolicyDto(Guid.NewGuid(), DateTime.Now),
            new DelinquentPolicyDto(Guid.NewGuid(), DateTime.Now)
        };
        _mockAdminService.Setup(s => s.GetDelinquentPoliciesAsync()).ReturnsAsync(policies);

        // Act
        var result = await _controller.GetDelinquentPolicies();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(policies);
    }

    [Fact]
    public async Task GetAllPremiumSchedules_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockAdminService.Setup(s => s.GetAllPremiumSchedulesAsync()).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.GetAllPremiumSchedules();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
        statusResult.Value.Should().Be("An error occurred while fetching premium schedules.");
    }

    [Fact]
    public async Task GetDelinquentPolicies_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockAdminService.Setup(s => s.GetDelinquentPoliciesAsync()).ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.GetDelinquentPolicies();

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var statusResult = result as ObjectResult;
        statusResult!.StatusCode.Should().Be(500);
        statusResult.Value.Should().Be("An error occurred while fetching delinquent policies.");
    }
}