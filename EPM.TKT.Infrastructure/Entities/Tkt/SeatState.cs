using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("SeatState", Schema = Constants.Tkt)]
internal class SeatState
{
    [Key]
    public byte SeatStateId { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
