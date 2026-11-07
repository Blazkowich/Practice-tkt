using System.Linq.Expressions;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Infrastructure.Entities.Tkt;

namespace EPM.TKT.Infrastructure.Entities.Mappings.Tkt;

internal static class EventMappingExtensions
{
    public static IQueryable<EventModel> MapToDomainModel(this IQueryable<Event> query)
        => query.Select(MapToDomainExpression);

    private static readonly Expression<Func<Event, EventModel>> MapToDomainExpression =
        e => new EventModel
        {
            EventId     = e.EventId,
            VenueId     = e.VenueId,
            Name        = e.Name,
            Description = e.Description,
            EventDate   = e.EventDate,
            DoorsOpenAt = e.DoorsOpenAt,
            EventStatus = e.EventStatus != null ? e.EventStatus.Name : string.Empty
        };
}
