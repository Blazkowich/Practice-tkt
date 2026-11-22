using EPM.TKT.App.UseCases.Payment;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Payment;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EPM.TKT.WebAPITests.Controllers;

[TestFixture]
public sealed class PaymentsControllerTests
{
    private Mock<ISender> _sender = null!;
    private PaymentsController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _sender = new Mock<ISender>();
        _controller = new PaymentsController(_sender.Object);
    }

    // ── GetPayment ───────────────────────────────────────────────────────────

    [Test]
    public async Task GetPayment_ReturnsOk_WhenFound()
    {
        var paymentId = Guid.NewGuid();
        var payment = new PaymentModel
        {
            PaymentId = paymentId,
            CartId = Guid.NewGuid(),
            Status = "Pending",
            TotalAmount = 120m
        };
        _sender.Setup(s => s.SendAsync(It.IsAny<GetPaymentQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<PaymentModel>.Ok(payment));

        var result = await _controller.GetPaymentAsync(paymentId, default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetPayment_ReturnsNotFound_WhenMissing()
    {
        _sender.Setup(s => s.SendAsync(It.IsAny<GetPaymentQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<PaymentModel>.Fail("Payment not found.", ErrorType.NotFound));

        var result = await _controller.GetPaymentAsync(Guid.NewGuid(), default);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    // ── Complete ─────────────────────────────────────────────────────────────

    [Test]
    public async Task Complete_ReturnsNoContent_WhenSuccessful()
    {
        _sender.Setup(s => s.SendAsync(It.IsAny<CompletePaymentCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Ok());

        var result = await _controller.CompleteAsync(Guid.NewGuid(), default);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Complete_ReturnsNotFound_WhenPaymentMissing()
    {
        _sender.Setup(s => s.SendAsync(It.IsAny<CompletePaymentCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Fail("Payment not found.", ErrorType.NotFound));

        var result = await _controller.CompleteAsync(Guid.NewGuid(), default);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    // ── Failed ───────────────────────────────────────────────────────────────

    [Test]
    public async Task Failed_ReturnsNoContent_WhenSuccessful()
    {
        _sender.Setup(s => s.SendAsync(It.IsAny<FailPaymentCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Ok());

        var result = await _controller.FailedAsync(Guid.NewGuid(), default);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Failed_ReturnsNotFound_WhenPaymentMissing()
    {
        _sender.Setup(s => s.SendAsync(It.IsAny<FailPaymentCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Fail("Payment not found.", ErrorType.NotFound));

        var result = await _controller.FailedAsync(Guid.NewGuid(), default);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }
}
