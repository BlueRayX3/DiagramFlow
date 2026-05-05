using Dapper;
using DiagramFlow.API.Data;
using DiagramFlow.API.Models;
using System.Linq;

namespace DiagramFlow.API.Repositories;

public interface IRoleRepository
{
    Task<IReadOnlyList<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateRoleRequest request);
    Task<bool> UpdateAsync(int id, UpdateRoleRequest request);
    Task<bool> DeleteAsync(int id);
}

public sealed class RoleRepository : IRoleRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RoleRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync()
    {
        const string sql = @"
            SELECT RoleID AS RoleId,
                   RoleName,
                   Description,
                   CanCreateProject,
                   CanDeleteProject,
                   CanManageUsers
            FROM Roles
            ORDER BY RoleName";

        using var connection = _connectionFactory.CreateConnection();
        var roles = await connection.QueryAsync<Role>(sql);
        return roles.ToList();
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT RoleID AS RoleId,
                   RoleName,
                   Description,
                   CanCreateProject,
                   CanDeleteProject,
                   CanManageUsers
            FROM Roles
            WHERE RoleID = @Id";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Role>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(CreateRoleRequest request)
    {
        const string sql = @"
            INSERT INTO Roles (RoleName, Description, CanCreateProject, CanDeleteProject, CanManageUsers)
            VALUES (@RoleName, @Description, @CanCreateProject, @CanDeleteProject, @CanManageUsers);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, request);
    }

    public async Task<bool> UpdateAsync(int id, UpdateRoleRequest request)
    {
        const string sql = @"
            UPDATE Roles
            SET RoleName = @RoleName,
                Description = @Description,
                CanCreateProject = @CanCreateProject,
                CanDeleteProject = @CanDeleteProject,
                CanManageUsers = @CanManageUsers
            WHERE RoleID = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            request.RoleName,
            request.Description,
            request.CanCreateProject,
            request.CanDeleteProject,
            request.CanManageUsers
        });

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Roles WHERE RoleID = @Id";
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { Id = id });
        return rows > 0;
    }
}
