using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using BillingService.Api.Controllers;
using BillingService.Api.DTO;
using BillingService.Api.Services;
using BillingService.Api.Models;

namespace BillingService.Api.Tests.Controllers;

public class PoliciesControllerTests
{
    private readonly Mock<IPoliciesService> _mockPoliciesService;
    private readonly PoliciesController _controller;

    public PoliciesControllerTests()
    {
        _mockPoliciesService = new Mock<IPoliciesService>();
        _controller = new PoliciesController(_mockPoliciesService.Object);
    }

    [Fact]
    public async Task GetPremiumSchedule_WithValidId_ReturnsOkWithResponse()
    {
        // Arrange
        var premiumScheduleId = Guid.NewGuid();
        var premiumSchedule = new PremiumSchedule
        {
            Id = premiumScheduleId,
            Status = "Overdue",
            Amount = 1.5e6m,
            DueDate = DateTime.UtcNow.AddDays(-5)
        };
        var expectedResponse = new PolicyResponse { PremiumSchedule = premiumSchedule, Success = true };

        _mockPoliciesService
            .Setup(s => s.GetPremiumSchedule(premiumScheduleId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetPremiumSchedule(premiumScheduleId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResponse);
    }
}