using DiagramFlow.API.Models;
using DiagramFlow.API.Repositories;

namespace DiagramFlow.API.Endpoints;

public static class DiagramsEndpoints
{
    public static IEndpointRouteBuilder MapDiagramEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/diagrams");

        group.MapGet("/", async (int? projectId, IDiagramRepository repository) =>
        {
            if (projectId.HasValue)
            {
                var filtered = await repository.GetByProjectIdAsync(projectId.Value);
                return Results.Ok(filtered);
            }

            var diagrams = await repository.GetAllAsync();
            return Results.Ok(diagrams);
        });

        group.MapGet("/{id:int}", async (int id, IDiagramRepository repository) =>
        {
            var diagram = await repository.GetByIdAsync(id);
            return diagram is null ? Results.NotFound() : Results.Ok(diagram);
        });

        group.MapPost("/", async (CreateDiagramRequest request, IDiagramRepository repository) =>
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return Results.BadRequest("Diagram title is required.");
            }

            var id = await repository.CreateAsync(request);
            return Results.Created($"/api/diagrams/{id}", new { id });
        });

        group.MapPut("/{id:int}", async (int id, UpdateDiagramRequest request, IDiagramRepository repository) =>
        {
            var updated = await repository.UpdateAsync(id, request);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IDiagramRepository repository) =>
        {
            var deleted = await repository.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
