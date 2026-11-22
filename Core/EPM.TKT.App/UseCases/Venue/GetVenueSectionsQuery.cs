using EPM.TKT.App.Repositories;
using EPM.TKT.Domain.Common;
using EPM.TKT.Domain.Models.Venue;
using EPM.TKT.Mediator.Abstractions;

namespace EPM.TKT.App.UseCases.Venue;

public sealed record GetVenueSectionsQuery(int VenueId) : IRequest<Result<IReadOnlyList<SectionModel>>>;

internal sealed class GetVenueSectionsHandler(IVenueRepository venues)
    : IRequestHandler<GetVenueSectionsQuery, Result<IReadOnlyList<SectionModel>>>
{
    public async Task<Result<IReadOnlyList<SectionModel>>> Handle(GetVenueSectionsQuery request, CancellationToken cancellationToken)
    {
        var sections = await venues.GetSectionsAsync(request.VenueId, cancellationToken);
        return sections.Count == 0 && request.VenueId > 0
            ? Result<IReadOnlyList<SectionModel>>.Fail("Venue not found.", ErrorType.NotFound)
            : Result<IReadOnlyList<SectionModel>>.Ok(sections);
    }
}
