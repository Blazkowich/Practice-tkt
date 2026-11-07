using EPM.TKT.App.UseCases.Event;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.WebApi.Contracts.Events;
using Microsoft.AspNetCore.Mvc;

namespace EPM.TKT.WebApi.Controllers;

/// <summary>Event catalogue and seat availability.</summary>
[ApiController]
[Route("api/events")]
public sealed class EventsController(ISender sender) : ControllerBase
{
    /// <summary>List all events.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>All events with their status.</returns>
    /// <response code="200">List of events (may be empty).</response>
    [HttpGet]
    [EndpointSummary("List all events")]
    [EndpointDescription("Returns every event in the system along with its status and venue reference.")]
    [ProducesResponseType(typeof(IReadOnlyList<EventHttpResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var events = await sender.Send(new GetEventsQuery(), ct);
        return Ok(events.Select(Map));
    }

    /// <summary>List seats in a section for a specific event.</summary>
    /// <param name="eventId">Event identifier.</param>
    /// <param name="sectionId">Section identifier within the event's venue.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Seats with their current state and available price options.</returns>
    /// <response code="200">Seat list (may be empty if no seats in section).</response>
    [HttpGet("{eventId:int}/sections/{sectionId:int}/seats")]
    [EndpointSummary("Get seats for an event section")]
    [EndpointDescription("Returns all seats in the given section for the event, each with its current state (Available/Booked/Sold) and applicable price options.")]
    [ProducesResponseType(typeof(IReadOnlyList<SeatHttpResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSeats(int eventId, int sectionId, CancellationToken ct)
    {
        var result = await sender.Send(new GetEventSectionSeatsQuery(eventId, sectionId), ct);
        return result.Match(
            onSuccess: seats => Ok(seats.Select(Map)),
            onFailure: err   => Problem(err));
    }

    private static EventHttpResponse Map(EventModel e)
        => new(e.EventId, e.Name, e.Description, e.EventDate, e.DoorsOpenAt, e.EventStatus);

    private static SeatHttpResponse Map(SeatAvailabilityModel s)
        => new(
            s.EventSeatId,
            s.VenueSeatId,
            s.SectionId,
            s.RowId,
            s.SeatNumber,
            new SeatStateHttpResponse(s.SeatState.Id, s.SeatState.Name),
            s.PriceOptions.Select(p => new PriceOptionHttpResponse(p.PriceId, p.Name, p.Amount, p.Currency)).ToList());

    private IActionResult Problem(Error error) => error.Type switch
    {
        ErrorType.NotFound => NotFound(new { error.Message }),
        _                  => BadRequest(new { error.Message })
    };
}