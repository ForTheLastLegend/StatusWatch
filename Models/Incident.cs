namespace StatusWatch.Models;

public class Incident
{
    public int Id { get; set; }
    public string Titre { get; set; } = "";
    public string? Description { get; set; }
    public string Statut { get; set; } = "investigating";
    public string Severite { get; set; } = "minor";
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public int ServiceId { get; set; }

    // rempli quand on JOIN avec services, sinon null
    public string? ServiceNom { get; set; }
}
