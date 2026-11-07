using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Venue;

public sealed record GetVenuesQuery : IRequest<IReadOnlyList<VenueModel>>;

internal sealed class GetVenuesHandler(IVenueRepository venues)
    : IRequestHandler<GetVenuesQuery, IReadOnlyList<VenueModel>>
{
    public Task<IReadOnlyList<VenueModel>> Handle(GetVenuesQuery request, CancellationToken cancellationToken)
        => venues.GetAllAsync(cancellationToken);
}
