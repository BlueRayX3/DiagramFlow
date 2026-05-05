using Dapper;
using DiagramFlow.API.Data;
using DiagramFlow.API.Models;
using System.Linq;

namespace DiagramFlow.API.Repositories;

public interface IDiagramRepository
{
    Task<IReadOnlyList<Diagram>> GetAllAsync();
    Task<IReadOnlyList<Diagram>> GetByProjectIdAsync(int projectId);
    Task<Diagram?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateDiagramRequest request);
    Task<bool> UpdateAsync(int id, UpdateDiagramRequest request);
    Task<bool> DeleteAsync(int id);
}

public sealed class DiagramRepository : IDiagramRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DiagramRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<Diagram>> GetAllAsync()
    {
        const string sql = @"
            SELECT DiagramID AS DiagramId,
                   ProjectID AS ProjectId,
                   CreatedBy,
                   Title,
                   DiagramType,
                   Content,
                   Version,
                   CreatedAt,
                   UpdatedAt
            FROM Diagrams
            ORDER BY UpdatedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        var diagrams = await connection.QueryAsync<Diagram>(sql);
        return diagrams.ToList();
    }

    public async Task<IReadOnlyList<Diagram>> GetByProjectIdAsync(int projectId)
    {
        const string sql = @"
            SELECT DiagramID AS DiagramId,
                   ProjectID AS ProjectId,
                   CreatedBy,
                   Title,
                   DiagramType,
                   Content,
                   Version,
                   CreatedAt,
                   UpdatedAt
            FROM Diagrams
            WHERE ProjectID = @Id
            ORDER BY UpdatedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        var diagrams = await connection.QueryAsync<Diagram>(sql, new { Id = projectId });
        return diagrams.ToList();
    }

    public async Task<Diagram?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT DiagramID AS DiagramId,
                   ProjectID AS ProjectId,
                   CreatedBy,
                   Title,
                   DiagramType,
                   Content,
                   Version,
                   CreatedAt,
                   UpdatedAt
            FROM Diagrams
            WHERE DiagramID = @Id";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Diagram>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(CreateDiagramRequest request)
    {
        const string sql = @"
            INSERT INTO Diagrams (ProjectID, CreatedBy, Title, DiagramType, Content, Version, CreatedAt, UpdatedAt)
            VALUES (@ProjectId, @CreatedBy, @Title, @DiagramType, @Content, 1, GETDATE(), GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, request);
    }

    public async Task<bool> UpdateAsync(int id, UpdateDiagramRequest request)
    {
        const string sql = @"
            UPDATE Diagrams
            SET Title = @Title,
                DiagramType = @DiagramType,
                Content = @Content,
                Version = @Version,
                UpdatedAt = GETDATE()
            WHERE DiagramID = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            request.Title,
            request.DiagramType,
            request.Content,
            request.Version
        });

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Diagrams WHERE DiagramID = @Id";
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { Id = id });
        return rows > 0;
    }
}
