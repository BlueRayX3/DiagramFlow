using Dapper;
using DiagramFlow.API.Data;
using DiagramFlow.API.Models;
using DiagramFlow.API.Security;
using System.Data;

namespace DiagramFlow.API.Repositories;

public interface IAuthRepository
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
}

public sealed class AuthRepository : IAuthRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AuthRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string existsSql = @"
            SELECT COUNT(1)
            FROM Users
            WHERE Username = @Username OR Email = @Email";

        var exists = await connection.ExecuteScalarAsync<int>(existsSql, new
        {
            request.Username,
            request.Email
        });

        if (exists > 0)
        {
            return new AuthResult(false, "Username or email already exists.", null);
        }

        var roleId = await ResolveDefaultRoleIdAsync(connection);
        if (!roleId.HasValue)
        {
            return new AuthResult(false, "No roles available.", null);
        }

        const string insertSql = @"
            INSERT INTO Users (Username, Email, PasswordHash, RoleID, FirstName, LastName, AvatarURL, IsActive)
            VALUES (@Username, @Email, @PasswordHash, @RoleId, @FirstName, @LastName, NULL, 1);
            SELECT CAST(SCOPE_IDENTITY() AS int);";

        var passwordHash = PasswordHasher.Hash(request.Password);
        var userId = await connection.ExecuteScalarAsync<int>(insertSql, new
        {
            request.Username,
            request.Email,
            PasswordHash = passwordHash,
            RoleId = roleId.Value,
            request.FirstName,
            request.LastName
        });

        var response = new AuthResponse(
            userId,
            request.Username,
            request.Email,
            request.FirstName,
            request.LastName
        );

        return new AuthResult(true, null, response);
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        const string userSql = @"
            SELECT UserID AS UserId,
                   Username,
                   Email,
                   PasswordHash,
                   FirstName,
                   LastName
            FROM Users
            WHERE Username = @Input OR Email = @Input";

        var user = await connection.QuerySingleOrDefaultAsync<AuthUserRow>(userSql, new
        {
            Input = request.UsernameOrEmail
        });

        if (user is null)
        {
            return new AuthResult(false, "Invalid credentials.", null);
        }

        var hashedInput = PasswordHasher.Hash(request.Password);
        var matches = string.Equals(user.PasswordHash, hashedInput, StringComparison.OrdinalIgnoreCase)
            || string.Equals(user.PasswordHash, request.Password, StringComparison.Ordinal);

        if (!matches)
        {
            return new AuthResult(false, "Invalid credentials.", null);
        }

        await connection.ExecuteAsync(
            "UPDATE Users SET LastLoginAt = GETDATE() WHERE UserID = @Id",
            new { Id = user.UserId }
        );

        var response = new AuthResponse(
            user.UserId,
            user.Username,
            user.Email,
            user.FirstName,
            user.LastName
        );

        return new AuthResult(true, null, response);
    }

    private static async Task<int?> ResolveDefaultRoleIdAsync(IDbConnection connection)
    {
        const string viewerSql = "SELECT TOP 1 RoleID FROM Roles WHERE RoleName = 'Viewer'";
        var viewerId = await connection.ExecuteScalarAsync<int?>(viewerSql);
        if (viewerId.HasValue)
        {
            return viewerId.Value;
        }

        const string fallbackSql = "SELECT TOP 1 RoleID FROM Roles ORDER BY RoleID";
        return await connection.ExecuteScalarAsync<int?>(fallbackSql);
    }

    private sealed record AuthUserRow(
        int UserId,
        string Username,
        string Email,
        string PasswordHash,
        string FirstName,
        string LastName
    );
}
