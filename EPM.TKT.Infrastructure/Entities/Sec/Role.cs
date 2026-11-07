using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Sec;

[Table("Role", Schema = Constants.Sec)]
internal class Role
{
    [Key]
    public byte RoleId { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
