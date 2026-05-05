using Dapper;
using DiagramFlow.API.Data;
using DiagramFlow.API.Models;
using System.Linq;

namespace DiagramFlow.API.Repositories;

public interface ICollaboratorRepository
{
    Task<IReadOnlyList<Collaborator>> GetByProjectIdAsync(int projectId);
    Task<int> CreateAsync(CreateCollaboratorRequest request);
    Task<bool> UpdateAsync(int id, UpdateCollaboratorRequest request);
    Task<bool> DeleteAsync(int id);
}

public sealed class CollaboratorRepository : ICollaboratorRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CollaboratorRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Collaborator>> GetByProjectIdAsync(int projectId)
    {
        const string sql = @"
            SELECT CollaboratorID AS CollaboratorId,
                   ProjectID AS ProjectId,
                   UserID AS UserId,
                   Permission,
                   InvitedAt,
                   AcceptedAt
            FROM Collaborators
            WHERE ProjectID = @Id
            ORDER BY InvitedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        var collaborators = await connection.QueryAsync<Collaborator>(sql, new { Id = projectId });
        return collaborators.ToList();
    }

    public async Task<int> CreateAsync(CreateCollaboratorRequest request)
    {
        const string sql = @"
            INSERT INTO Collaborators (ProjectID, UserID, Permission, InvitedAt)
            VALUES (@ProjectId, @UserId, @Permission, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, request);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCollaboratorRequest request)
    {
        const string sql = @"
            UPDATE Collaborators
            SET Permission = @Permission,
                AcceptedAt = @AcceptedAt
            WHERE CollaboratorID = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            request.Permission,
            request.AcceptedAt
        });

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Collaborators WHERE CollaboratorID = @Id";
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { Id = id });
        return rows > 0;
    }
}
