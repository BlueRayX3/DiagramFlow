using DiagramFlow.API.Models;
using DiagramFlow.API.Repositories;

namespace DiagramFlow.API.Endpoints;

public static class DiagramHistoryEndpoints
{
    public static IEndpointRouteBuilder MapDiagramHistoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/diagram-history");

        group.MapGet("/", async (int? diagramId, IDiagramHistoryRepository repository) =>
        {
            if (!diagramId.HasValue)
            {
                return Results.BadRequest("diagramId query parameter is required.");
            }

            var history = await repository.GetByDiagramIdAsync(diagramId.Value);
            return Results.Ok(history);
        });

        group.MapPost("/", async (CreateDiagramHistoryRequest request, IDiagramHistoryRepository repository) =>
        {
            if (string.IsNullOrWhiteSpace(request.PreviousContent))
            {
                return Results.BadRequest("PreviousContent is required.");
            }

            var id = await repository.CreateAsync(request);
            return Results.Created($"/api/diagram-history/{id}", new { id });
        });

        return app;
    }
}
