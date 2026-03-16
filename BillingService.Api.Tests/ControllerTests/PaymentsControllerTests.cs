using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BillingService.Api.Controllers;
using BillingService.Api.Services;
using BillingService.Api.DTO;
using Microsoft.Extensions.Logging;
using BillingService.Api.Models;

namespace BillingService.Api.Tests.Controllers;

public class PaymentsControllerTests
{
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly Mock<ILogger<PaymentsController>> _mockLogger;
    private readonly PaymentsController _controller;

    public PaymentsControllerTests()
    {
        _mockPaymentService = new Mock<IPaymentService>();
        _mockLogger = new Mock<ILogger<PaymentsController>>();
        _controller = new PaymentsController(_mockPaymentService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task AttemptPayment_WithValidRequest_ReturnsOkWithResult()
    {
        // Arrange
        var request = new PaymentRequest { PolicyId = Guid.NewGuid(), Amount = 100.0m, ShouldSucceed = true };
        var expectedResult = new PaymentResponse { AttemptId = Guid.NewGuid(), Success = true };
        var correlationId = "test-correlation-id";

        _mockPaymentService.Setup(s => s.ProcessPaymentAsync(request)).ReturnsAsync(expectedResult);

        // Set up HttpContext with correlation ID
        var httpContext = new DefaultHttpContext();
        httpContext.Items["CorrelationId"] = correlationId;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Act
        var result = await _controller.AttemptPayment(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        Assert.Equal(expectedResult, okResult!.Value);

        // Verify logging
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Received payment attempt for policy {request.PolicyId}") && o.ToString()!.Contains(correlationId)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Payment recorded. AttemptId={expectedResult.AttemptId}") && o.ToString()!.Contains(correlationId)),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task RetryPayment_WithExistingPaymentId_ReturnsOkWithPayment()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var expectedPayment = new PaymentAttempt { Id = paymentId, Success = true };
        var expectedResponse = new PaymentResponse { AttemptId = paymentId, Success = true };

        _mockPaymentService.Setup(s => s.RetryPaymentAsync(paymentId)).ReturnsAsync(expectedPayment);

        // Act
        var result = await _controller.RetryPayment(paymentId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.Should().BeEquivalentTo(expectedResponse);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Payment retry successful. AttemptId={expectedPayment.Id}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task RetryPayment_WithNonExistingPaymentId_ReturnsNotFound()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        _mockPaymentService.Setup(s => s.RetryPaymentAsync(paymentId)).ReturnsAsync((PaymentAttempt?)null);

        // Act
        var result = await _controller.RetryPayment(paymentId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Payment retry failed. PaymentId={paymentId} not found.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}