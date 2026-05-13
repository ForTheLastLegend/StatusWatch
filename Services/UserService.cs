using System.Data;
using Dapper;
using StatusWatch.Models;

namespace StatusWatch.Services;

public class UserService
{
    private readonly IDbConnection _db;

    public UserService(IDbConnection db)
    {
        _db = db;
    }

    public User? GetByEmail(string email)
    {
        const string sql = @"
            SELECT id, nom, email, password_hash AS PasswordHash, role
            FROM users
            WHERE email = @email";

        return _db.QueryFirstOrDefault<User>(sql, new { email });
    }

    public bool EmailExists(string email)
    {
        const string sql = "SELECT COUNT(*) FROM users WHERE email = @email";
        return _db.ExecuteScalar<int>(sql, new { email }) > 0;
    }

    public int Create(User user)
    {
        const string sql = @"
            INSERT INTO users (nom, email, password_hash, role)
            VALUES (@Nom, @Email, @PasswordHash, @Role)
            RETURNING id";

        return _db.ExecuteScalar<int>(sql, user);
    }
}
