using Dapper;
using DiagramFlow.API.Data;
using DiagramFlow.API.Models;
using System.Linq;

namespace DiagramFlow.API.Repositories;

public interface ITemplateRepository
{
    Task<IReadOnlyList<DiagramTemplate>> GetAllAsync();
    Task<DiagramTemplate?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateTemplateRequest request);
    Task<bool> UpdateAsync(int id, UpdateTemplateRequest request);
    Task<bool> DeleteAsync(int id);
}

public sealed class TemplateRepository : ITemplateRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TemplateRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<DiagramTemplate>> GetAllAsync()
    {
        const string sql = @"
            SELECT TemplateID AS TemplateId,
                   Name,
                   Description,
                   DiagramType,
                   Content,
                   IsSystemTemplate,
                   CreatedBy,
                   CreatedAt
            FROM Diagram_Templates
            ORDER BY CreatedAt DESC";

        using var connection = _connectionFactory.CreateConnection();
        var templates = await connection.QueryAsync<DiagramTemplate>(sql);
        return templates.ToList();
    }

    public async Task<DiagramTemplate?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT TemplateID AS TemplateId,
                   Name,
                   Description,
                   DiagramType,
                   Content,
                   IsSystemTemplate,
                   CreatedBy,
                   CreatedAt
            FROM Diagram_Templates
            WHERE TemplateID = @Id";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<DiagramTemplate>(sql, new { Id = id });
    }

    public async Task<int> CreateAsync(CreateTemplateRequest request)
    {
        const string sql = @"
            INSERT INTO Diagram_Templates (Name, Description, DiagramType, Content, IsSystemTemplate, CreatedBy, CreatedAt)
            VALUES (@Name, @Description, @DiagramType, @Content, @IsSystemTemplate, @CreatedBy, GETDATE());
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, request);
    }

    public async Task<bool> UpdateAsync(int id, UpdateTemplateRequest request)
    {
        const string sql = @"
            UPDATE Diagram_Templates
            SET Name = @Name,
                Description = @Description,
                DiagramType = @DiagramType,
                Content = @Content,
                IsSystemTemplate = @IsSystemTemplate
            WHERE TemplateID = @Id";

        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new
        {
            Id = id,
            request.Name,
            request.Description,
            request.DiagramType,
            request.Content,
            request.IsSystemTemplate
        });

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = "DELETE FROM Diagram_Templates WHERE TemplateID = @Id";
        using var connection = _connectionFactory.CreateConnection();
        var rows = await connection.ExecuteAsync(sql, new { Id = id });
        return rows > 0;
    }
}
