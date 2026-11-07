using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("Offer", Schema = Constants.Tkt)]
internal class Offer
{
    [Key]
    public int OfferId { get; set; }

    public int EventId { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public DateTime SaleStartAt { get; set; }

    public DateTime? SaleEndAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(EventId))]
    public Event? Event { get; set; }
}
