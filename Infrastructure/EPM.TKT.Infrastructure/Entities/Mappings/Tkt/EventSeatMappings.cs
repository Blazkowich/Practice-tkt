using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class EventSeatMappingExtensions
{
    public static IQueryable<EventSeatModel> MapToDomainModel(this IQueryable<EventSeat> query)
        => query.Select(MapToDomainExpression);

    public static EventSeatModel MapToDomainModel(this EventSeat es) => new()
    {
        EventSeatId = es.EventSeatId,
        EventId = es.EventId,
        VenueSeatId = es.VenueSeatId,
        PriceId = es.PriceId,
        SeatState = es.SeatState != null ? es.SeatState.Name : string.Empty,
        HeldByUserId = es.HeldByUserId,
        HoldUntil = es.HoldUntil,
        CreatedAt = es.CreatedAt
    };

    private static readonly Expression<Func<EventSeat, EventSeatModel>> MapToDomainExpression =
        es => new EventSeatModel
        {
            EventSeatId = es.EventSeatId,
            EventId = es.EventId,
            VenueSeatId = es.VenueSeatId,
            PriceId = es.PriceId,
            SeatState = es.SeatState != null ? es.SeatState.Name : string.Empty,
            HeldByUserId = es.HeldByUserId,
            HoldUntil = es.HoldUntil,
            CreatedAt = es.CreatedAt
        };
}
