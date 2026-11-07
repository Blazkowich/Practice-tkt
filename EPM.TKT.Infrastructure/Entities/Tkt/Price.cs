using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("Price", Schema = Constants.Tkt)]
internal class Price
{
    [Key]
    public int PriceId { get; set; }

    public int OfferId { get; set; }

    public int? SectionId { get; set; }

    public int? RowId { get; set; }

    public byte PriceCategoryId { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [MaxLength(3)]
    [Column(TypeName = "char(3)")]
    public string Currency { get; set; } = "USD";

    [ForeignKey(nameof(OfferId))]
    public Offer? Offer { get; set; }

    [ForeignKey(nameof(SectionId))]
    public Section? Section { get; set; }

    [ForeignKey(nameof(RowId))]
    public Row? Row { get; set; }

    [ForeignKey(nameof(PriceCategoryId))]
    public PriceCategory? PriceCategory { get; set; }
}
