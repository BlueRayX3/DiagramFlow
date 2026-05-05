using DiagramFlow.API.Models;
using DiagramFlow.API.Repositories;

namespace DiagramFlow.API.Endpoints;

public static class TemplatesEndpoints
{
    public static IEndpointRouteBuilder MapTemplateEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/templates");

        group.MapGet("/", async (ITemplateRepository repository) =>
        {
            var templates = await repository.GetAllAsync();
            return Results.Ok(templates);
        });

        group.MapGet("/{id:int}", async (int id, ITemplateRepository repository) =>
        {
            var template = await repository.GetByIdAsync(id);
            return template is null ? Results.NotFound() : Results.Ok(template);
        });

        group.MapPost("/", async (CreateTemplateRequest request, ITemplateRepository repository) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest("Template name is required.");
            }

            var id = await repository.CreateAsync(request);
            return Results.Created($"/api/templates/{id}", new { id });
        });

        group.MapPut("/{id:int}", async (int id, UpdateTemplateRequest request, ITemplateRepository repository) =>
        {
            var updated = await repository.UpdateAsync(id, request);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, ITemplateRepository repository) =>
        {
            var deleted = await repository.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        });

        return app;
    }
}
