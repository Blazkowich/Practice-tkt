using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("SeatType", Schema = Constants.Tkt)]
internal class SeatType
{
    [Key]
    public byte SeatTypeId { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
