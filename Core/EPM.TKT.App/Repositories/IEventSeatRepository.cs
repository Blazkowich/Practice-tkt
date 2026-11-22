using EPM.TKT.Domain.Enums;
using EPM.TKT.Domain.Models.Event;

namespace EPM.TKT.App.Repositories;

public interface IEventSeatRepository
{
    Task<IReadOnlyList<EventSeatModel>> GetAllAsync(
        int eventId,
        CancellationToken ct = default
    );
    Task<EventSeatModel?> GetByIdAsync(
        long eventSeatId,
        CancellationToken ct = default
    );
    Task<long> CreateAsync(
        int eventId,
        int venueSeatId,
        int? priceId,
        CancellationToken ct = default
    );
    Task HoldAsync(
        long eventSeatId,
        Guid userId,
        DateTime holdUntil,
        CancellationToken ct = default
    );
    Task ReleaseAsync(
        long eventSeatId,
        CancellationToken ct = default
    );
    Task UpdateStateAsync(
        long eventSeatId,
        SeatState seatState,
        CancellationToken ct = default
    );
    Task DeleteAsync(
        long eventSeatId,
        CancellationToken ct = default
    );
}
