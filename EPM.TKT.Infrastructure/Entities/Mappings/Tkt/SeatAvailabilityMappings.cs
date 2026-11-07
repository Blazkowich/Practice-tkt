using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class SeatAvailabilityMappingExtensions
{
    /// <summary>
    /// Projects EventSeats to SeatAvailabilityModel. PriceOptions must be supplied as a pre-fetched
    /// list because correlated sub-queries with IReadOnlyList properties cannot be translated to SQL.
    /// </summary>
    public static IQueryable<SeatAvailabilityModel> MapToSeatAvailability(
        this IQueryable<EventSeat> query,
        IReadOnlyList<PriceOptionModel> priceOptions)
        => query.Select(es => new SeatAvailabilityModel
        {
            EventSeatId  = es.EventSeatId,
            VenueSeatId  = es.VenueSeatId,
            SectionId    = es.VenueSeat != null ? es.VenueSeat.SectionId : 0,
            RowId        = es.VenueSeat != null ? es.VenueSeat.RowId : null,
            SeatNumber   = es.VenueSeat != null ? es.VenueSeat.SeatNumber : null,
            SeatState    = es.SeatState != null
                ? new SeatStateModel { Id = es.SeatStateId, Name = es.SeatState.Name }
                : new SeatStateModel { Id = es.SeatStateId },
            PriceOptions = priceOptions
        });
}
