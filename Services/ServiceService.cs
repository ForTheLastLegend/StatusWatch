using System.Data;
using Dapper;
using StatusWatch.Models;

namespace StatusWatch.Services;

public class ServiceService
{
    private readonly IDbConnection _db;

    public ServiceService(IDbConnection db)
    {
        _db = db;
    }

    public List<Service> GetAll()
    {
        const string sql = @"
            SELECT id, nom, description, url, categorie, statut
            FROM services
            ORDER BY nom";

        return _db.Query<Service>(sql).ToList();
    }

    public Service? GetById(int id)
    {
        const string sql = @"
            SELECT id, nom, description, url, categorie, statut
            FROM services
            WHERE id = @id";

        return _db.QueryFirstOrDefault<Service>(sql, new { id });
    }
}
