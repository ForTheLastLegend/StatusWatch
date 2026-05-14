using System.ComponentModel.DataAnnotations;

namespace StatusWatch.Models;

public class Incident
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le titre est requis")]
    [StringLength(200)]
    public string Titre { get; set; } = "";

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    [StringLength(20)]
    public string Statut { get; set; } = "investigating";

    [Required]
    [StringLength(20)]
    public string Severite { get; set; } = "minor";

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime DateDebut { get; set; }

    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime? DateFin { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Service requis")]
    public int ServiceId { get; set; }

    // rempli quand on JOIN avec services, sinon null
    public string? ServiceNom { get; set; }
}
