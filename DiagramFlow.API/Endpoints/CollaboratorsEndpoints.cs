using DiagramFlow.API.Models;
using DiagramFlow.API.Repositories;

namespace DiagramFlow.API.Endpoints;

public static class CollaboratorsEndpoints
{
    public static IEndpointRouteBuilder MapCollaboratorEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/collaborators");

        group.MapGet("/", async (int? projectId, ICollaboratorRepository repository) =>
        {
            if (!projectId.HasValue)
            {
                return Results.BadRequest("projectId query parameter is required.");
            }

            var collaborators = await repository.GetByProjectIdAsync(projectId.Value);
            return Results.Ok(collaborators);
        });

        group.MapPost("/", async (CreateCollaboratorRequest request, ICollaboratorRepository repository) =>
        {
            if (string.IsNullOrWhiteSpace(request.Permission))
            {
                return Results.BadRequest("Permission is required.");
            }

            var id = await repository.CreateAsync(request);
            return Results.Created($"/api/collaborators/{id}", new { id });
        });

        group.MapPut("/{id:int}", async (int id, UpdateCollaboratorRequest request, ICollaboratorRepository repository) =>
        {
            var updated = await repository.UpdateAsync(id, request);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, ICollaboratorRepository repository) =>
        {
            var deleted = await repository.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
