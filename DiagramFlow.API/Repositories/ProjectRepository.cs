using Dapper;
using DiagramFlow.API.Data;
using DiagramFlow.API.Models;
using System.Linq;

namespace DiagramFlow.API.Repositories;

public interface IProjectRepository
{
    Task<IReadOnlyList<ProjectListItem>> GetPublicAsync();
    Task<ProjectDetails?> GetByIdAsync(int id);
    Task<IReadOnlyList<ProjectListItem>> GetByUserIdAsync(int userId);
    Task<int> CreateAsync(CreateProjectRequest request);
    Task<bool> UpdateAsync(int id, UpdateProjectRequest request);
    Task<bool> DeleteAsync(int id);
}

public sealed class ProjectRepository : IProjectRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ProjectRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<ProjectListItem>> GetPublicAsync()
    {
        const string sql = @"
            SELECT p.ProjectID AS ProjectId,
                   p.Name,
                   p.Description,
                   u.Username AS OwnerName,
                   p.IsPublic,
                   p.CreatedAt,
                   p.UpdatedAt
            FROM Projects p
            INNER JOIN Users u ON p.OwnerID = u.UserID
            WHERE p.IsPublic = 1
            ORDER BY p.UpdatedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        var projects = await connection.QueryAsync<ProjectListItem>(sql);
        return projects.ToList();
    }

    public async Task<ProjectDetails?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT p.ProjectID AS ProjectId,
                   p.OwnerID AS OwnerId,
                   u.Username AS OwnerName,
                   p.Name,
                   p.Description,
                   p.IsPublic,
                   p.Settings,
                   p.CreatedAt,
                   p.UpdatedAt
            FROM Projects p
            INNER JOIN Users u ON p.OwnerID = u.UserID
            WHERE p.ProjectID = @Id";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<ProjectDetails>(sql, new { Id = id });
    }

    public async Task<IReadOnlyList<ProjectListItem>> GetByUserIdAsync(int userId)
    {
        const string sql = @"
            SELECT p.ProjectID AS ProjectId,
                   p.Name,
                   p.Description,
                   u.Username AS OwnerName,
                   p.IsPublic,
                   p.CreatedAt,
                   p.UpdatedAt
            FROM Projects p
            INNER JOIN Users u ON p.OwnerID = u.UserID
            WHERE u.UserID = @Id
            ORDER BY p.CreatedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        var projects = await connection.QueryAsync<ProjectListItem>(sql, new { Id = userId });
        return projects.ToList();
    }

    public async Task<int> CreateAsync(CreateProjectRequest request)
    {
        const string sql = @"
            INSERT INTO Projects (OwnerID, Name, Description, IsPublic, Settings, CreatedAt, UpdatedAt)
            VALUES (@OwnerId, @Name, @Description, @IsPublic, @Settings, GETDATE(), GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _connectionFactory.CreateConnection();
        var ownerId = await ResolveOwnerIdAsync(request.OwnerId, connection);
        if (!ownerId.HasValue)
        {
            throw new InvalidOperationException("No users available to own this project.");
        }

        var isPublic = request.IsPublic ?? true;
        return await connection.ExecuteScalarAsync<int>(sql, new
        {
            OwnerId = ownerId.Value,
            request.Name,
            request.Description,
            IsPublic = isPublic,
            request.Settings
        });
    }

    private static async Task<int?> ResolveOwnerIdAsync(int? requestedOwnerId, SqlConnection connection)
    {
        if (requestedOwnerId.HasValue)
        {
            const string existsSql = "SELECT COUNT(1) FROM Users WHERE UserID = @Id";
            var exists = await connection.ExecuteScalarAsync<int>(existsSql, new { Id = requestedOwnerId });
            if (exists > 0)
            {
                return requestedOwnerId.Value;
            }
        }

        const string fallbackSql = "SELECT TOP 1 UserID FROM Users ORDER BY UserID";
        return await connection.ExecuteScalarAsync<int?>(fallbackSql);
    }

    public async Task<bool> UpdateAsync(int id, UpdateProjectRequest request)
    {
        const string sql = @"
            UPDATE Projects
            SET Name = @Name,
                Description = @Description,
                IsPublic = COALESCE(@IsPublic, IsPublic),
                Settings = @Settings,
                UpdatedAt = GETDATE()
            WHERE ProjectID = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            request.Name,
            request.Description,
            request.IsPublic,
            request.Settings
        });

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Projects WHERE ProjectID = @Id";
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { Id = id });
        return rows > 0;
    }
}
