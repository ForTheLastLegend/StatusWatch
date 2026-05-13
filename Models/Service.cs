namespace StatusWatch.Models;

public class Service
{
    public int Id { get; set; }
    public string Nom { get; set; } = "";
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? Categorie { get; set; }
    public string Statut { get; set; } = "operational";
}
