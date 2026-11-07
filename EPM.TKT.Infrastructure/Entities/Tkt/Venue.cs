using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EPM.TKT.Infrastructure.Entities.Tkt;

[Table("Venue", Schema = Constants.Tkt)]
internal class Venue
{
    [Key]
    public int VenueId { get; set; }

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}
