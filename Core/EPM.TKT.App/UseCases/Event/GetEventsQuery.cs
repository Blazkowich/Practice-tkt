using EPM.TKT.App.Cache;
using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Mediator.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace EPM.TKT.App.UseCases.Event;

public sealed record GetEventsQuery : IRequest<IReadOnlyList<EventModel>>;

internal sealed class GetEventsHandler(IEventRepository events, IMemoryCache cache)
    : IRequestHandler<GetEventsQuery, IReadOnlyList<EventModel>>
{
    public async Task<IReadOnlyList<EventModel>> HandleAsync(GetEventsQuery request, CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(EventCacheKeys.EventsList, out IReadOnlyList<EventModel>? cached) && cached is not null)
            return cached;

        var result = await events.GetAllAsync(cancellationToken);

        cache.Set(EventCacheKeys.EventsList, result, TimeSpan.FromMinutes(10));

        return result;
    }
}
