using DiagramFlow.API.Models;
using DiagramFlow.API.Repositories;

namespace DiagramFlow.API.Endpoints;

public static class RolesEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/roles");

        group.MapGet("/", async (IRoleRepository repository) =>
        {
            var roles = await repository.GetAllAsync();
            return Results.Ok(roles);
        });

        group.MapGet("/{id:int}", async (int id, IRoleRepository repository) =>
        {
            var role = await repository.GetByIdAsync(id);
            return role is null ? Results.NotFound() : Results.Ok(role);
        });

        group.MapPost("/", async (CreateRoleRequest request, IRoleRepository repository) =>
        {
            if (string.IsNullOrWhiteSpace(request.RoleName))
            {
                return Results.BadRequest("Role name is required.");
            }

            var id = await repository.CreateAsync(request);
            return Results.Created($"/api/roles/{id}", new { id });
        });

        group.MapPut("/{id:int}", async (int id, UpdateRoleRequest request, IRoleRepository repository) =>
        {
            var updated = await repository.UpdateAsync(id, request);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IRoleRepository repository) =>
        {
            var deleted = await repository.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
