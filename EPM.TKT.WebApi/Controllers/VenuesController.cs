using EPM.TKT.App.UseCases.Venue;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Mediator.Abstractions;
using EPM.TKT.WebApi.Contracts.Venues;
using Microsoft.AspNetCore.Mvc;

namespace EPM.TKT.WebApi.Controllers;

/// <summary>Venue catalogue and manifest queries.</summary>
[ApiController]
[Route("api/venues")]
public sealed class VenuesController(ISender sender) : ControllerBase
{
    /// <summary>List all active venues.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>All active venues.</returns>
    /// <response code="200">List of venues (may be empty).</response>
    [HttpGet]
    [EndpointSummary("List all venues")]
    [EndpointDescription("Returns every active venue in the system.")]
    [ProducesResponseType(typeof(IReadOnlyList<VenueHttpResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var venues = await sender.Send(new GetVenuesQuery(), ct);
        return Ok(venues.Select(Map));
    }

    /// <summary>List all sections in a venue.</summary>
    /// <param name="venueId">Venue identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Sections with their seat type.</returns>
    /// <response code="200">List of sections.</response>
    /// <response code="404">Venue not found.</response>
    [HttpGet("{venueId:int}/sections")]
    [EndpointSummary("Get sections for a venue")]
    [EndpointDescription("Returns all sections (Reserved or General Admission) belonging to the given venue.")]
    [ProducesResponseType(typeof(IReadOnlyList<SectionHttpResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSections(int venueId, CancellationToken ct)
    {
        var result = await sender.Send(new GetVenueSectionsQuery(venueId), ct);
        return result.Match(
            onSuccess: sections => Ok(sections.Select(Map)),
            onFailure: err      => Problem(err));
    }

    private static VenueHttpResponse Map(VenueModel v)
        => new(v.VenueId, v.Name, v.City, v.Country);

    private static SectionHttpResponse Map(SectionModel s)
        => new(s.SectionId, s.Name, s.SeatType);

    private IActionResult Problem(Error error) => error.Type switch
    {
        ErrorType.NotFound => NotFound(new { error.Message }),
        _                  => BadRequest(new { error.Message })
    };
}