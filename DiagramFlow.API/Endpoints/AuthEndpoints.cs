using DiagramFlow.API.Models;
using DiagramFlow.API.Repositories;
using Microsoft.AspNetCore.Http;

namespace DiagramFlow.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (RegisterRequest request, IAuthRepository repository) =>
        {
            if (string.IsNullOrWhiteSpace(request.Username)
                || string.IsNullOrWhiteSpace(request.Email)
                || string.IsNullOrWhiteSpace(request.Password)
                || string.IsNullOrWhiteSpace(request.FirstName)
                || string.IsNullOrWhiteSpace(request.LastName))
            {
                return Results.BadRequest("All fields are required.");
            }

            var result = await repository.RegisterAsync(request);
            return result.Success
                ? Results.Created("/api/auth/login", result.User)
                : Results.BadRequest(result.Error);
        });

        group.MapPost("/login", async (LoginRequest request, IAuthRepository repository) =>
        {
            if (string.IsNullOrWhiteSpace(request.UsernameOrEmail)
                || string.IsNullOrWhiteSpace(request.Password))
            {
                return Results.BadRequest("Username/email and password are required.");
            }

            var result = await repository.LoginAsync(request);
            return result.Success
                ? Results.Ok(result.User)
                : Results.Problem("Invalid credentials.", statusCode: StatusCodes.Status401Unauthorized);
        });

        return app;
    }
}
