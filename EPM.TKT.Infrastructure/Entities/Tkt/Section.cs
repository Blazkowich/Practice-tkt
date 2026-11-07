using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("Section", Schema = Constants.Tkt)]
internal class Section
{
    [Key]
    public int SectionId { get; set; }

    public int VenueId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public byte SeatTypeId { get; set; }

    public int SortOrder { get; set; }

    [ForeignKey(nameof(VenueId))]
    public Venue? Venue { get; set; }

    [ForeignKey(nameof(SeatTypeId))]
    public SeatType? SeatType { get; set; }
}
