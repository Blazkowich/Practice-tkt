using EPM.TKT.App.UseCases.Cart;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Cart;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.WebApi.Contracts.Orders;
using Microsoft.AspNetCore.Mvc;

namespace EPM.TKT.WebApi.Controllers;

/// <summary>Shopping cart and booking operations.</summary>
[ApiController]
[Route("api/orders")]
public sealed class OrdersController(ISender sender) : ControllerBase
{
    /// <summary>Get the current state of a cart.</summary>
    /// <param name="cartId">Client-generated cart UUID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Cart with items and total amount.</returns>
    /// <response code="200">Cart found.</response>
    /// <response code="404">Cart does not exist yet.</response>
    [HttpGet("carts/{cartId:guid}")]
    [EndpointSummary("Get cart contents")]
    [EndpointDescription("Returns the cart with all items, prices and total. The cartId is generated and stored client-side.")]
    [ProducesResponseType(typeof(CartHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCart(Guid cartId, CancellationToken ct)
    {
        var cart = await sender.Send(new GetCartQuery(cartId), ct);
        return cart is null ? NotFound(new { Message = "Cart not found." }) : Ok(Map(cart));
    }

    /// <summary>Add a seat to the cart.</summary>
    /// <param name="cartId">Client-generated cart UUID. The cart is created on first use.</param>
    /// <param name="request">Event, seat (VenueSeatId) and price to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Updated cart state with total amount.</returns>
    /// <response code="200">Seat added — updated cart returned.</response>
    /// <response code="400">Seat or price not found.</response>
    [HttpPost("carts/{cartId:guid}")]
    [EndpointSummary("Add a seat to the cart")]
    [EndpointDescription("Adds the specified seat to the cart, creating the cart if it does not exist. Returns the full cart state.")]
    [ProducesResponseType(typeof(CartHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToCart(
        Guid cartId, [FromBody] AddToCartHttpRequest request, CancellationToken ct)
    {
        var result = await sender.Send(
            new AddToCartCommand(cartId, request.EventId, request.SeatId, request.PriceId), ct);

        return result.Match(
            onSuccess: cart => Ok(Map(cart)),
            onFailure: err  => Problem(err));
    }

    /// <summary>Remove a seat from the cart.</summary>
    /// <param name="cartId">Cart identifier.</param>
    /// <param name="eventId">Event the seat belongs to.</param>
    /// <param name="seatId">VenueSeatId of the seat to remove.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="204">Seat removed (or was not in the cart — idempotent).</response>
    /// <response code="400">Invalid parameters.</response>
    [HttpDelete("carts/{cartId:guid}/events/{eventId:int}/seats/{seatId:int}")]
    [EndpointSummary("Remove a seat from the cart")]
    [EndpointDescription("Removes the given seat from the cart. Operation is idempotent — no error if the seat is not present.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveFromCart(
        Guid cartId, int eventId, int seatId, CancellationToken ct)
    {
        var result = await sender.Send(new RemoveFromCartCommand(cartId, eventId, seatId), ct);
        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: err => Problem(err));
    }

    /// <summary>Book all seats in the cart.</summary>
    /// <param name="cartId">Cart to book.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Payment identifier used to complete or cancel the booking.</returns>
    /// <response code="200">Seats moved to Booked state — PaymentId returned.</response>
    /// <response code="400">Cart is empty or not found.</response>
    [HttpPut("carts/{cartId:guid}/book")]
    [EndpointSummary("Book all seats in the cart")]
    [EndpointDescription("Moves every seat in the cart to Booked state, creates an Order, and returns a PaymentId. Use the PaymentId to complete or fail the payment.")]
    [ProducesResponseType(typeof(BookHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Book(Guid cartId, CancellationToken ct)
    {
        var result = await sender.Send(new BookCartCommand(cartId), ct);
        return result.Match(
            onSuccess: paymentId => Ok(new BookHttpResponse(paymentId)),
            onFailure: err       => Problem(err));
    }

    private static CartHttpResponse Map(CartModel c) => new(
        c.CartId,
        c.EventId,
        c.Items.Select(i => new CartItemHttpResponse(
            i.EventSeatId, i.VenueSeatId, i.SeatNumber, i.RowName,
            i.SectionName, i.PriceId, i.PriceName, i.Amount, i.Currency)).ToList(),
        c.TotalAmount);

    private IActionResult Problem(Error error) => error.Type switch
    {
        ErrorType.NotFound => NotFound(new { error.Message }),
        _                  => BadRequest(new { error.Message })
    };
}