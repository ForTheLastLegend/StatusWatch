using System.Data;
using Dapper;
using StatusWatch.Models;

namespace StatusWatch.Services;

public class IncidentUpdateService
{
    private readonly IDbConnection _db;

    public IncidentUpdateService(IDbConnection db)
    {
        _db = db;
    }

    public List<IncidentUpdate> GetByIncident(int incidentId)
    {
        const string sql = @"
            SELECT id, incident_id AS IncidentId, message, date_creation AS DateCreation
            FROM incident_updates
            WHERE incident_id = @incidentId
            ORDER BY date_creation DESC";

        return _db.Query<IncidentUpdate>(sql, new { incidentId }).ToList();
    }

    public int Create(IncidentUpdate u)
    {
        const string sql = @"
            INSERT INTO incident_updates (incident_id, message)
            VALUES (@IncidentId, @Message)
            RETURNING id";

        return _db.ExecuteScalar<int>(sql, u);
    }
}
