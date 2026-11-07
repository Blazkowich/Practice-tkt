using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("EventStatus", Schema = Constants.Tkt)]
internal class EventStatus
{
    [Key]
    public byte EventStatusId { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
