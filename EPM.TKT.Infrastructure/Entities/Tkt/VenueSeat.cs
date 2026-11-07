using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("VenueSeat", Schema = Constants.Tkt)]
internal class VenueSeat
{
    [Key]
    public int VenueSeatId { get; set; }

    public int SectionId { get; set; }

    public int? RowId { get; set; }

    [MaxLength(20)]
    public string? SeatNumber { get; set; }

    public int SortOrder { get; set; }

    [ForeignKey(nameof(SectionId))]
    public Section? Section { get; set; }

    [ForeignKey(nameof(RowId))]
    public Row? Row { get; set; }
}
