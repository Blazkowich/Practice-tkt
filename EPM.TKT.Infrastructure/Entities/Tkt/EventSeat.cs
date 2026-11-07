using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("EventSeat", Schema = Constants.Tkt)]
internal class EventSeat
{
    [Key]
    public long EventSeatId { get; set; }

    public int EventId { get; set; }

    public int VenueSeatId { get; set; }

    public int? PriceId { get; set; }

    public byte SeatStateId { get; set; }

    public Guid? HeldByUserId { get; set; }

    public DateTime? HoldUntil { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }

    [ForeignKey(nameof(VenueSeatId))]
    public VenueSeat? VenueSeat { get; set; }

    [ForeignKey(nameof(PriceId))]
    public Price? Price { get; set; }

    [ForeignKey(nameof(SeatStateId))]
    public SeatState? SeatState { get; set; }
}
