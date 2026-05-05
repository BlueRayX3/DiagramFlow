namespace DiagramFlow.API.Models;

public sealed record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName
);

public sealed record LoginRequest(
    string UsernameOrEmail,
    string Password
);

public sealed record AuthResponse(
    int UserId,
    string Username,
    string Email,
    string FirstName,
    string LastName
);

public sealed record AuthResult(
    bool Success,
    string? Error,
    AuthResponse? User
);
