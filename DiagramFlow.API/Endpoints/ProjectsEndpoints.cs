using DiagramFlow.API.Models;
using DiagramFlow.API.Repositories;

namespace DiagramFlow.API.Endpoints;

public static class ProjectsEndpoints
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/projects");

        group.MapGet("/", async (IProjectRepository repository) =>
        {
            var projects = await repository.GetPublicAsync();
            return Results.Ok(projects);
        });

        group.MapGet("/{id:int}", async (int id, IProjectRepository repository) =>
        {
            var project = await repository.GetByIdAsync(id);
            return project is null ? Results.NotFound() : Results.Ok(project);
        });

        group.MapPost("/", async (CreateProjectRequest request, IProjectRepository repository) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest("Project name is required.");
            }

            try
            {
                var id = await repository.CreateAsync(request);
                return Results.Created($"/api/projects/{id}", new { id });
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch
            {
                return Results.BadRequest("Unable to create project.");
            }
        });

        group.MapPut("/{id:int}", async (int id, UpdateProjectRequest request, IProjectRepository repository) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Results.BadRequest("Project name is required.");
            }

            var updated = await repository.UpdateAsync(id, request);
            return updated ? Results.NoContent() : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IProjectRepository repository) =>
        {
            try
            {
                var deleted = await repository.DeleteAsync(id);
                return deleted ? Results.NoContent() : Results.NotFound();
            }
            catch
            {
                return Results.BadRequest("Unable to delete project with linked data.");
            }
        });

        app.MapGet("/api/user-projects/{userId:int}", async (int userId, IProjectRepository repository) =>
        {
            var projects = await repository.GetByUserIdAsync(userId);
            return Results.Ok(projects);
        });

        return app;
    }
}
