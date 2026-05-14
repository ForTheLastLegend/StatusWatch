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

    public List<Incident> GetActive()
    {
        const string sql = @"
            SELECT i.id, i.titre, i.description, i.statut, i.severite,
                   i.date_debut AS DateDebut, i.date_fin AS DateFin,
                   i.service_id AS ServiceId, s.nom AS ServiceNom
            FROM incidents i
            JOIN services s ON s.id = i.service_id
            WHERE i.statut <> 'resolved'
            ORDER BY i.date_debut DESC";

        return _db.Query<Incident>(sql).ToList();
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

    public List<Incident> Search(string? search, string? statut, string? severite, int pageNumber, int pageSize)
    {
        var sql = @"
            SELECT i.id, i.titre, i.description, i.statut, i.severite,
                   i.date_debut AS DateDebut, i.date_fin AS DateFin,
                   i.service_id AS ServiceId, s.nom AS ServiceNom
            FROM incidents i
            JOIN services s ON s.id = i.service_id
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            sql += " AND (i.titre ILIKE @search OR i.description ILIKE @search)";
            parameters.Add("search", $"%{search}%");
        }
        if (!string.IsNullOrWhiteSpace(statut))
        {
            sql += " AND i.statut = @statut";
            parameters.Add("statut", statut);
        }
        if (!string.IsNullOrWhiteSpace(severite))
        {
            sql += " AND i.severite = @severite";
            parameters.Add("severite", severite);
        }

        sql += " ORDER BY i.date_debut DESC LIMIT @pageSize OFFSET @offset";
        parameters.Add("pageSize", pageSize);
        parameters.Add("offset", (pageNumber - 1) * pageSize);

        return _db.Query<Incident>(sql, parameters).ToList();
    }

    public int Count(string? search, string? statut, string? severite)
    {
        var sql = "SELECT COUNT(*) FROM incidents i WHERE 1=1";
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(search))
        {
            sql += " AND (i.titre ILIKE @search OR i.description ILIKE @search)";
            parameters.Add("search", $"%{search}%");
        }
        if (!string.IsNullOrWhiteSpace(statut))
        {
            sql += " AND i.statut = @statut";
            parameters.Add("statut", statut);
        }
        if (!string.IsNullOrWhiteSpace(severite))
        {
            sql += " AND i.severite = @severite";
            parameters.Add("severite", severite);
        }

        return _db.ExecuteScalar<int>(sql, parameters);
    }

    public int Create(Incident i)
    {
        const string sql = @"
            INSERT INTO incidents (titre, description, statut, severite, date_debut, date_fin, service_id)
            VALUES (@Titre, @Description, @Statut, @Severite, @DateDebut, @DateFin, @ServiceId)
            RETURNING id";

        var newId = _db.ExecuteScalar<int>(sql, i);
        RecalculateServiceStatus(i.ServiceId);
        return newId;
    }

    public void Update(Incident i)
    {
        // on lit l'ancien service_id pour gérer le cas où l'incident change de service
        var oldServiceId = _db.QueryFirstOrDefault<int?>(
            "SELECT service_id FROM incidents WHERE id = @Id", new { i.Id });

        const string sql = @"
            UPDATE incidents
            SET titre = @Titre, description = @Description, statut = @Statut,
                severite = @Severite, date_debut = @DateDebut, date_fin = @DateFin,
                service_id = @ServiceId
            WHERE id = @Id";

        _db.Execute(sql, i);

        RecalculateServiceStatus(i.ServiceId);
        if (oldServiceId.HasValue && oldServiceId.Value != i.ServiceId)
        {
            RecalculateServiceStatus(oldServiceId.Value);
        }
    }

    public void Delete(int id)
    {
        var serviceId = _db.QueryFirstOrDefault<int?>(
            "SELECT service_id FROM incidents WHERE id = @id", new { id });

        _db.Execute("DELETE FROM incidents WHERE id = @id", new { id });

        if (serviceId.HasValue)
        {
            RecalculateServiceStatus(serviceId.Value);
        }
    }

    public bool HasActiveIncidents(int serviceId)
    {
        const string sql = @"
            SELECT COUNT(*) FROM incidents
            WHERE service_id = @serviceId AND statut <> 'resolved'";

        return _db.ExecuteScalar<int>(sql, new { serviceId }) > 0;
    }

    private void RecalculateServiceStatus(int serviceId)
    {
        const string activesSql = @"
            SELECT severite FROM incidents
            WHERE service_id = @serviceId AND statut <> 'resolved'";

        var actives = _db.Query<string>(activesSql, new { serviceId }).ToList();

        string newStatut;
        if (actives.Count == 0)
        {
            newStatut = "operational";
        }
        else if (actives.Any(s => s == "critical" || s == "major"))
        {
            newStatut = "outage";
        }
        else
        {
            newStatut = "degraded";
        }

        _db.Execute("UPDATE services SET statut = @newStatut WHERE id = @serviceId",
            new { newStatut, serviceId });
    }
}
