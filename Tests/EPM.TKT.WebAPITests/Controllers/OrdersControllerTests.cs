using System.Security.Claims;
using EPM.TKT.App.UseCases.Cart;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Cart;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.WebApi.Contracts.Requests.Orders;
using EPM.TKT.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace EPM.TKT.WebAPITests.Controllers;

[TestFixture]
public sealed class OrdersControllerTests
{
    private Mock<ISender> _sender = null!;
    private OrdersController _controller = null!;
    private Guid _userId;
    private Guid _cartId;

    [SetUp]
    public void SetUp()
    {
        _sender = new Mock<ISender>();
        _controller = new OrdersController(_sender.Object);
        _userId = Guid.NewGuid();
        _cartId = Guid.NewGuid();
    }

    private void SetAuthenticatedUser()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, _userId.ToString())
                ]))
            }
        };
    }

    private void SetUnauthenticatedUser()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    private static CartModel FakeCart(Guid cartId) => new()
    {
        CartId = cartId, EventId = 1, Items = [], TotalAmount = 50m
    };

    // ── GetCart ──────────────────────────────────────────────────────────────

    [Test]
    public async Task GetCart_ReturnsOk_WhenCartExists()
    {
        _sender.Setup(s => s.SendAsync(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(FakeCart(_cartId));

        var result = await _controller.GetCartAsync(_cartId, default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task GetCart_ReturnsNotFound_WhenCartDoesNotExist()
    {
        _sender.Setup(s => s.SendAsync(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((CartModel?)null);

        var result = await _controller.GetCartAsync(_cartId, default);

        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    // ── AddToCart ────────────────────────────────────────────────────────────

    [Test]
    public async Task AddToCart_ReturnsOk_WhenSeatAdded()
    {
        SetAuthenticatedUser();
        _sender.Setup(s => s.SendAsync(It.IsAny<AddToCartCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<CartModel>.Ok(FakeCart(_cartId)));

        var result = await _controller.AddToCartAsync(
            _cartId, new AddToCartHttpRequest(EventId: 1, SeatId: 10, PriceId: 2), default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task AddToCart_ReturnsBadRequest_WhenSeatUnavailable()
    {
        SetAuthenticatedUser();
        _sender.Setup(s => s.SendAsync(It.IsAny<AddToCartCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<CartModel>.Fail("Seat not available."));

        var result = await _controller.AddToCartAsync(
            _cartId, new AddToCartHttpRequest(EventId: 1, SeatId: 10, PriceId: 2), default);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task AddToCart_ReturnsUnauthorized_WhenUserClaimMissing()
    {
        SetUnauthenticatedUser();

        var result = await _controller.AddToCartAsync(
            _cartId, new AddToCartHttpRequest(EventId: 1, SeatId: 10, PriceId: 2), default);

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    // ── RemoveFromCart ───────────────────────────────────────────────────────

    [Test]
    public async Task RemoveFromCart_ReturnsNoContent_WhenSuccessful()
    {
        _sender.Setup(s => s.SendAsync(It.IsAny<RemoveFromCartCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Ok());

        var result = await _controller.RemoveFromCartAsync(_cartId, eventId: 1, seatId: 10, default);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    // ── Book ─────────────────────────────────────────────────────────────────

    [Test]
    public async Task Book_ReturnsOk_WithPaymentId()
    {
        SetAuthenticatedUser();
        var paymentId = Guid.NewGuid();
        _sender.Setup(s => s.SendAsync(It.IsAny<BookCartCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Ok(paymentId));

        var result = await _controller.BookAsync(_cartId, default);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Book_ReturnsUnauthorized_WhenUserClaimMissing()
    {
        SetUnauthenticatedUser();

        var result = await _controller.BookAsync(_cartId, default);

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Book_ReturnsBadRequest_WhenCartIsEmpty()
    {
        SetAuthenticatedUser();
        _sender.Setup(s => s.SendAsync(It.IsAny<BookCartCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<Guid>.Fail("Cart is empty."));

        var result = await _controller.BookAsync(_cartId, default);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}
