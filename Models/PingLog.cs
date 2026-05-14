namespace StatusWatch.Models;

public class PingLog
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public string Statut { get; set; } = "up";
    public int? LatenceMs { get; set; }
    public DateTime CheckedAt { get; set; }
}
