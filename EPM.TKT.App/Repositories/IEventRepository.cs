using EPM.TKT.Domain.Models.Event;

namespace EPM.TKT.App.Repositories;

public interface IEventRepository
{
    Task<IReadOnlyList<EventModel>> GetAllAsync(CancellationToken ct = default);
    Task<EventModel?> GetByIdAsync(int eventId, CancellationToken ct = default);
    Task<int> CreateAsync(int venueId, string name, string? description, DateTime eventDate, DateTime? doorsOpenAt, byte eventStatusId, CancellationToken ct = default);
    Task UpdateAsync(int eventId, string name, string? description, DateTime eventDate, DateTime? doorsOpenAt, byte eventStatusId, CancellationToken ct = default);
    Task DeleteAsync(int eventId, CancellationToken ct = default);
    Task<IReadOnlyList<SeatAvailabilityModel>> GetSeatsAsync(int eventId, int sectionId, CancellationToken ct = default);
}
