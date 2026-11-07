using EPM.TKT.Domain.Models.Event;

namespace EPM.TKT.App.Repositories;

public interface IEventRepository
{
    Task<IReadOnlyList<EventModel>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SeatAvailabilityModel>> GetSeatsAsync(int eventId, int sectionId, CancellationToken ct = default);
}
