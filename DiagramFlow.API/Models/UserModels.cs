namespace DiagramFlow.API.Models;

public sealed record User(
    int UserId,
    string Username,
    string Email,
    string PasswordHash,
    int RoleId,
    string FirstName,
    string LastName,
    string? AvatarUrl,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);

public sealed record CreateUserRequest(
    string Username,
    string Email,
    string PasswordHash,
    int RoleId,
    string FirstName,
    string LastName,
    string? AvatarUrl,
    bool IsActive
);

public sealed record UpdateUserRequest(
    string Email,
    string PasswordHash,
    int RoleId,
    string FirstName,
    string LastName,
    string? AvatarUrl,
    bool IsActive,
    DateTime? LastLoginAt
);
