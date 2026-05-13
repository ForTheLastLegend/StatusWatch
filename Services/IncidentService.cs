using System.Data;
using Dapper;
using StatusWatch.Models;

namespace StatusWatch.Services;

public class IncidentService
{
    private readonly IDbConnection _db;

    public IncidentService(IDbConnection db)
    {
        _db = db;
    }

    public List<Incident> GetAll()
    {
        const string sql = @"
            SELECT i.id, i.titre, i.description, i.statut, i.severite,
                   i.date_debut AS DateDebut, i.date_fin AS DateFin,
                   i.service_id AS ServiceId, s.nom AS ServiceNom
            FROM incidents i
            JOIN services s ON s.id = i.service_id
            ORDER BY i.date_debut DESC";

        return _db.Query<Incident>(sql).ToList();
    }

    public Incident? GetById(int id)
    {
        const string sql = @"
            SELECT i.id, i.titre, i.description, i.statut, i.severite,
                   i.date_debut AS DateDebut, i.date_fin AS DateFin,
                   i.service_id AS ServiceId, s.nom AS ServiceNom
            FROM incidents i
            JOIN services s ON s.id = i.service_id
            WHERE i.id = @id";

        return _db.QueryFirstOrDefault<Incident>(sql, new { id });
    }

    public List<Incident> GetByService(int serviceId)
    {
        const string sql = @"
            SELECT id, titre, description, statut, severite,
                   date_debut AS DateDebut, date_fin AS DateFin,
                   service_id AS ServiceId
            FROM incidents
            WHERE service_id = @serviceId
            ORDER BY date_debut DESC";

        return _db.Query<Incident>(sql, new { serviceId }).ToList();
    }
}
