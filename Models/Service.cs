using System.ComponentModel.DataAnnotations;

namespace StatusWatch.Models;

public class Service
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Le nom est requis.")]
    [StringLength(100)]
    public string Nom { get; set; } = "";

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(300)]
    public string? Url { get; set; }

    [StringLength(50)]
    public string? Categorie { get; set; }

    [Required]
    [StringLength(20)]
    public string Statut { get; set; } = "operational";
}
