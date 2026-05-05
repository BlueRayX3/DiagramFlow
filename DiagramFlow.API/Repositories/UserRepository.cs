using Dapper;
using DiagramFlow.API.Data;
using DiagramFlow.API.Models;
using System.Linq;

namespace DiagramFlow.API.Repositories;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateUserRequest request);
    Task<bool> UpdateAsync(int id, UpdateUserRequest request);
    Task<bool> DeleteAsync(int id);
}

public sealed class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        const string sql = @"
            SELECT UserID AS UserId,
                   Username,
                   Email,
                   PasswordHash,
                   RoleID AS RoleId,
                   FirstName,
                   LastName,
                   AvatarURL AS AvatarUrl,
                   IsActive,
                   CreatedAt,
                   LastLoginAt
            FROM Users
            ORDER BY CreatedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        var users = await connection.QueryAsync<User>(sql);
        return users.ToList();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT UserID AS UserId,
                   Username,
                   Email,
                   PasswordHash,
                   RoleID AS RoleId,
                   FirstName,
                   LastName,
                   AvatarURL AS AvatarUrl,
                   IsActive,
                   CreatedAt,
                   LastLoginAt
            FROM Users
            WHERE UserID = @Id";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(CreateUserRequest request)
    {
        const string sql = @"
            INSERT INTO Users (Username, Email, PasswordHash, RoleID, FirstName, LastName, AvatarURL, IsActive)
            VALUES (@Username, @Email, @PasswordHash, @RoleId, @FirstName, @LastName, @AvatarUrl, @IsActive);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, request);
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserRequest request)
    {
        const string sql = @"
            UPDATE Users
            SET Email = @Email,
                PasswordHash = @PasswordHash,
                RoleID = @RoleId,
                FirstName = @FirstName,
                LastName = @LastName,
                AvatarURL = @AvatarUrl,
                IsActive = @IsActive,
                LastLoginAt = @LastLoginAt
            WHERE UserID = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            request.Email,
            request.PasswordHash,
            request.RoleId,
            request.FirstName,
            request.LastName,
            request.AvatarUrl,
            request.IsActive,
            request.LastLoginAt
        });

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Users WHERE UserID = @Id";
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { Id = id });
        return rows > 0;
    }
}
