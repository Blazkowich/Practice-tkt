using EPM.TKT.App.UseCases.Payment;
using EPM.TKT.Domain.Common;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.WebApi.Contracts.Payments;
using Microsoft.AspNetCore.Mvc;

namespace EPM.TKT.WebApi.Controllers;

/// <summary>Payment status and fulfilment.</summary>
[ApiController]
[Route("api/payments")]
public sealed class PaymentsController(ISender sender) : ControllerBase
{
    /// <summary>Get the status of a payment.</summary>
    /// <param name="paymentId">Order / payment identifier (returned by book).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Payment with inferred status and total amount.</returns>
    /// <response code="200">Payment found.</response>
    /// <response code="404">Payment not found.</response>
    [HttpGet("{paymentId:guid}")]
    [EndpointSummary("Get payment status")]
    [EndpointDescription("Returns the payment with its inferred status: Pending (seats booked), Completed (seats sold), or Failed (seats released).")]
    [ProducesResponseType(typeof(PaymentHttpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayment(Guid paymentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPaymentQuery(paymentId), ct);
        return result.Match(
            onSuccess: p  => Ok(new PaymentHttpResponse(p.PaymentId, p.CartId, p.Status, p.TotalAmount)),
            onFailure: err => Problem(err));
    }

    /// <summary>Mark a payment as complete and move seats to Sold.</summary>
    /// <param name="paymentId">Payment identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="204">Payment completed — all Booked seats moved to Sold.</response>
    /// <response code="404">Payment not found.</response>
    [HttpPost("{paymentId:guid}/complete")]
    [EndpointSummary("Complete a payment")]
    [EndpointDescription("Transitions all Booked seats in the payment to the Sold state.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(Guid paymentId, CancellationToken ct)
    {
        var result = await sender.Send(new CompletePaymentCommand(paymentId), ct);
        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: err => Problem(err));
    }

    /// <summary>Mark a payment as failed and release seats back to Available.</summary>
    /// <param name="paymentId">Payment identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="204">Payment failed — all Booked seats returned to Available.</response>
    /// <response code="404">Payment not found.</response>
    [HttpPost("{paymentId:guid}/failed")]
    [EndpointSummary("Fail a payment")]
    [EndpointDescription("Releases all Booked seats in the payment back to Available state.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Failed(Guid paymentId, CancellationToken ct)
    {
        var result = await sender.Send(new FailPaymentCommand(paymentId), ct);
        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: err => Problem(err));
    }

    private IActionResult Problem(Error error) => error.Type switch
    {
        ErrorType.NotFound => NotFound(new { error.Message }),
        _                  => BadRequest(new { error.Message })
    };
}