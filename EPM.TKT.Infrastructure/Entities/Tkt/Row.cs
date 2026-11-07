using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("Row", Schema = Constants.Tkt)]
internal class Row
{
    [Key]
    public int RowId { get; set; }

    public int SectionId { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    [ForeignKey(nameof(SectionId))]
    public Section? Section { get; set; }
}
