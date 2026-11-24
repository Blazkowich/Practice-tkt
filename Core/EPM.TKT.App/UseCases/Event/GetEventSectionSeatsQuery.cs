using EPM.TKT.App.Cache;
using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Mediator.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace EPM.TKT.App.UseCases.Event;

public sealed record GetEventSectionSeatsQuery(int EventId, int SectionId)
    : IRequest<Result<IReadOnlyList<SeatAvailabilityModel>>>;

internal sealed class GetEventSectionSeatsHandler(IEventRepository events, IMemoryCache cache)
    : IRequestHandler<GetEventSectionSeatsQuery, Result<IReadOnlyList<SeatAvailabilityModel>>>
{
    public async Task<Result<IReadOnlyList<SeatAvailabilityModel>>> HandleAsync(
        GetEventSectionSeatsQuery request, CancellationToken cancellationToken)
    {
        var generation = cache.GetOrCreate(EventCacheKeys.EventSeatGeneration(request.EventId), _ => 0);
        var seatKey = EventCacheKeys.EventSectionSeats(request.EventId, request.SectionId, generation);

        if (cache.TryGetValue(seatKey, out IReadOnlyList<SeatAvailabilityModel>? cached) && cached is not null)
            return Result<IReadOnlyList<SeatAvailabilityModel>>.Ok(cached);

        var seats = await events.GetSeatsAsync(request.EventId, request.SectionId, cancellationToken);

        cache.Set(seatKey, seats, TimeSpan.FromMinutes(5));

        return Result<IReadOnlyList<SeatAvailabilityModel>>.Ok(seats);
    }
}
