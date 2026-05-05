using Dapper;
using DiagramFlow.API.Data;
using DiagramFlow.API.Models;
using System.Linq;

namespace DiagramFlow.API.Repositories;

public interface IDiagramHistoryRepository
{
    Task<IReadOnlyList<DiagramHistory>> GetByDiagramIdAsync(int diagramId);
    Task<int> CreateAsync(CreateDiagramHistoryRequest request);
}

public sealed class DiagramHistoryRepository : IDiagramHistoryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DiagramHistoryRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<DiagramHistory>> GetByDiagramIdAsync(int diagramId)
    {
        const string sql = @"
            SELECT HistoryID AS HistoryId,
                   DiagramID AS DiagramId,
                   ChangedBy,
                   PreviousContent,
                   ChangeDescription,
                   CreatedAt
            FROM Diagram_History
            WHERE DiagramID = @Id
            ORDER BY CreatedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        var history = await connection.QueryAsync<DiagramHistory>(sql, new { Id = diagramId });
        return history.ToList();
    }

    public async Task<int> CreateAsync(CreateDiagramHistoryRequest request)
    {
        const string sql = @"
            INSERT INTO Diagram_History (DiagramID, ChangedBy, PreviousContent, ChangeDescription, CreatedAt)
            VALUES (@DiagramId, @ChangedBy, @PreviousContent, @ChangeDescription, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, request);
    }
}
