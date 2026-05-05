using Microsoft.Data.SqlClient;

namespace DiagramFlow.API.Data;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
}
