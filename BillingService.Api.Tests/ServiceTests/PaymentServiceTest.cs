using Xunit;
using Moq;
using FluentAssertions;
using BillingService.Api.Services;
using BillingService.Api.Repositories;
using BillingService.Api.DTO;
using BillingService.Api.Models;
using Microsoft.Extensions.Logging;

namespace BillingService.Api.Tests.ServiceTests;

public class PaymentServiceTest
{
    private readonly Mock<IPaymentPolicyRepository> _mockRepository;
    private readonly Mock<ILogger<PaymentService>> _mockLogger;
    private readonly PaymentService _service;

    public PaymentServiceTest()
    {
        _mockRepository = new Mock<IPaymentPolicyRepository>();
        _mockLogger = new Mock<ILogger<PaymentService>>();
        _service = new PaymentService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsSuccessResponse_WhenSuccessful()
    {
        // Arrange
        var request = new PaymentRequest { PolicyId = Guid.NewGuid(), Amount = 100.0m, ShouldSucceed = true };

        // Act
        var result = await _service.ProcessPaymentAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.AttemptId.Should().NotBeEmpty();
        _mockRepository.Verify(r => r.AddPaymentAttemptAsync(It.Is<PaymentAttempt>(p => p.PolicyId == request.PolicyId && p.Amount == request.Amount)), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsFailureResponse_WhenExceptionThrown()
    {
        // Arrange
        var request = new PaymentRequest { PolicyId = Guid.NewGuid(), Amount = 100.0m, ShouldSucceed = false };
        _mockRepository.Setup(r => r.AddPaymentAttemptAsync(It.IsAny<PaymentAttempt>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.ProcessPaymentAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.AttemptId.Should().Be(request.PolicyId);
        _mockRepository.Verify(r => r.AddPaymentAttemptAsync(It.IsAny<PaymentAttempt>()), Times.Once);
    }

    [Fact]
    public async Task RetryPaymentAsync_ReturnsPaymentAttempt_WhenFound()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var payment = new PaymentAttempt { Id = paymentId, Success = false, RetryCount = 0 };
        _mockRepository.Setup(r => r.GetPaymentAttemptAsync(paymentId)).ReturnsAsync(payment);

        // Act
        var result = await _service.RetryPaymentAsync(paymentId);

        // Assert
        result.Should().Be(payment);
        result!.RetryCount.Should().Be(1);
        _mockRepository.Verify(r => r.GetPaymentAttemptAsync(paymentId), Times.Once);
        _mockRepository.Verify(r => r.UpdatePaymentAttemptAsync(payment), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RetryPaymentAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetPaymentAttemptAsync(paymentId)).ReturnsAsync((PaymentAttempt?)null);

        // Act
        var result = await _service.RetryPaymentAsync(paymentId);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetPaymentAttemptAsync(paymentId), Times.Once);
        _mockRepository.Verify(r => r.UpdatePaymentAttemptAsync(It.IsAny<PaymentAttempt>()), Times.Never);
    }

    [Fact]
    public async Task RetryPaymentAsync_ReturnsFailureAttempt_WhenExceptionThrown()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetPaymentAttemptAsync(paymentId)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _service.RetryPaymentAsync(paymentId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(paymentId);
        result.Success.Should().BeFalse();
        _mockRepository.Verify(r => r.GetPaymentAttemptAsync(paymentId), Times.Once);
    }
}