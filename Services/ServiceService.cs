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

    public int Create(Service s)
    {
        const string sql = @"
            INSERT INTO services (nom, description, url, categorie, statut)
            VALUES (@Nom, @Description, @Url, @Categorie, @Statut)
            RETURNING id";

        return _db.ExecuteScalar<int>(sql, s);
    }

    public void Update(Service s)
    {
        const string sql = @"
            UPDATE services
            SET nom = @Nom, description = @Description, url = @Url,
                categorie = @Categorie, statut = @Statut
            WHERE id = @Id";

        _db.Execute(sql, s);
    }

    public void Delete(int id)
    {
        const string sql = "DELETE FROM services WHERE id = @id";
        _db.Execute(sql, new { id });
    }
}
