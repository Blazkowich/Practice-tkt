using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("Event", Schema = Constants.Tkt)]
internal class Event
{
    [Key]
    public int EventId { get; set; }

    public int VenueId { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    public DateTime? DoorsOpenAt { get; set; }

    public byte EventStatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(VenueId))]
    public Venue? Venue { get; set; }

    [ForeignKey(nameof(EventStatusId))]
    public EventStatus? EventStatus { get; set; }
}
