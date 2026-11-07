using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("PriceCategory", Schema = Constants.Tkt)]
internal class PriceCategory
{
    [Key]
    public byte PriceCategoryId { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
