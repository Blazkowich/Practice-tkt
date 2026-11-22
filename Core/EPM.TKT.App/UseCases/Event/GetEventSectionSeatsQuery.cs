using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Event;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Event;

public sealed record GetEventSectionSeatsQuery(int EventId, int SectionId)
    : IRequest<Result<IReadOnlyList<SeatAvailabilityModel>>>;

internal sealed class GetEventSectionSeatsHandler(IEventRepository events)
    : IRequestHandler<GetEventSectionSeatsQuery, Result<IReadOnlyList<SeatAvailabilityModel>>>
{
    public async Task<Result<IReadOnlyList<SeatAvailabilityModel>>> Handle(
        GetEventSectionSeatsQuery request, CancellationToken cancellationToken)
    {
        var seats = await events.GetSeatsAsync(request.EventId, request.SectionId, cancellationToken);
        return Result<IReadOnlyList<SeatAvailabilityModel>>.Ok(seats);
    }
}
