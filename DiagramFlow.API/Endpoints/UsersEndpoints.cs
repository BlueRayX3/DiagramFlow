using DiagramFlow.API.Models;
using DiagramFlow.API.Repositories;

namespace DiagramFlow.API.Endpoints;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        group.MapGet("/", async (IUserRepository repository) =>
        {
            var users = await repository.GetAllAsync();
            return Results.Ok(users);
        });

        group.MapGet("/{id:int}", async (int id, IUserRepository repository) =>
        {
            var user = await repository.GetByIdAsync(id);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        group.MapPost("/", async (CreateUserRequest request, IUserRepository repository) =>
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Email))
            {
                return Results.BadRequest("Username and email are required.");
            }

            var id = await repository.CreateAsync(request);
            return Results.Created($"/api/users/{id}", new { id });
        });

        group.MapPut("/{id:int}", async (int id, UpdateUserRequest request, IUserRepository repository) =>
        {
            var updated = await repository.UpdateAsync(id, request);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IUserRepository repository) =>
        {
            var deleted = await repository.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
