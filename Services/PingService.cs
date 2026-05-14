using System.Data;
using Dapper;
using StatusWatch.Models;

namespace StatusWatch.Services;

public class PingService
{
    private readonly IDbConnection _db;

    public PingService(IDbConnection db)
    {
        _db = db;
    }

    public void Log(int serviceId, string statut, int? latenceMs)
    {
        const string sql = @"
            INSERT INTO ping_logs (service_id, statut, latence_ms)
            VALUES (@serviceId, @statut, @latenceMs)";

        _db.Execute(sql, new { serviceId, statut, latenceMs });
    }

    // recalcule services.statut en croisant le dernier ping et les incidents actifs
    // priorite aux incidents critical/major (outage) ; sinon un ping down passe a outage ;
    // sinon un minor actif passe a degraded ; sinon operational
    public void UpdateServiceStatusFromPing(int serviceId, string pingStatut)
    {
        const string activesSql = @"
            SELECT severite FROM incidents
            WHERE service_id = @serviceId AND statut <> 'resolved'";

        var actives = _db.Query<string>(activesSql, new { serviceId }).ToList();

        string newStatut;
        if (actives.Any(s => s == "critical" || s == "major"))
        {
            newStatut = "outage";
        }
        else if (pingStatut == "down")
        {
            newStatut = "outage";
        }
        else if (actives.Any(s => s == "minor"))
        {
            newStatut = "degraded";
        }
        else
        {
            newStatut = "operational";
        }

        _db.Execute("UPDATE services SET statut = @newStatut WHERE id = @serviceId",
            new { newStatut, serviceId });
    }

    // pourcentage de pings "up" sur les @days derniers jours ; null si aucune donnee
    public double? GetUptimePercent(int serviceId, int days = 30)
    {
        const string sql = @"
            SELECT
                COUNT(*) AS total,
                COUNT(*) FILTER (WHERE statut = 'up') AS up_count
            FROM ping_logs
            WHERE service_id = @serviceId
              AND checked_at >= NOW() - make_interval(days => @days)";

        var row = _db.QuerySingle<(long total, long up_count)>(sql, new { serviceId, days });
        if (row.total == 0)
        {
            return null;
        }
        return Math.Round((double)row.up_count * 100.0 / row.total, 2);
    }

    // derniers pings du service (par defaut sur les @hours dernieres heures), du plus recent au plus ancien
    public List<PingLog> GetLatencyHistory(int serviceId, int hours = 24)
    {
        const string sql = @"
            SELECT
                id AS Id,
                service_id AS ServiceId,
                statut AS Statut,
                latence_ms AS LatenceMs,
                checked_at AS CheckedAt
            FROM ping_logs
            WHERE service_id = @serviceId
              AND checked_at >= NOW() - make_interval(hours => @hours)
            ORDER BY checked_at DESC";

        return _db.Query<PingLog>(sql, new { serviceId, hours }).ToList();
    }
}
