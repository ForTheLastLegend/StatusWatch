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

        return _db.ExecuteScalar<int>(sql, i);
    }

    public void Update(Incident i)
    {
        const string sql = @"
            UPDATE incidents
            SET titre = @Titre, description = @Description, statut = @Statut,
                severite = @Severite, date_debut = @DateDebut, date_fin = @DateFin,
                service_id = @ServiceId
            WHERE id = @Id";

        _db.Execute(sql, i);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM incidents WHERE id = @id";
        _db.Execute(sql, new { id });
    }
}
