using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DiagramFlow.API.Data;

public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    }

    public SqlConnection CreateConnection() => new SqlConnection(_connectionString);
}
