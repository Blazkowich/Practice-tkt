using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Event;

public sealed record GetEventsQuery : IRequest<IReadOnlyList<EventModel>>;

internal sealed class GetEventsHandler(IEventRepository events)
    : IRequestHandler<GetEventsQuery, IReadOnlyList<EventModel>>
{
    public Task<IReadOnlyList<EventModel>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        => events.GetAllAsync(cancellationToken);
}
