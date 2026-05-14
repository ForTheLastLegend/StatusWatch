using System.ComponentModel.DataAnnotations;

namespace StatusWatch.Models;

public class IncidentUpdate
{
    public int Id { get; set; }

    public int IncidentId { get; set; }

    [Required(ErrorMessage = "Le message est requis")]
    [StringLength(2000)]
    public string Message { get; set; } = "";

    public DateTime DateCreation { get; set; }
}
