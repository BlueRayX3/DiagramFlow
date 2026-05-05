namespace DiagramFlow.API.Models;

public sealed record Project(
    int ProjectId,
    int OwnerId,
    string Name,
    string? Description,
    bool IsPublic,
    string? Settings,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public sealed record ProjectListItem(
    int ProjectId,
    string Name,
    string? Description,
    string OwnerName,
    bool IsPublic,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public sealed record ProjectDetails(
    int ProjectId,
    int OwnerId,
    string OwnerName,
    string Name,
    string? Description,
    bool IsPublic,
    string? Settings,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public sealed record CreateProjectRequest(
    string Name,
    string? Description,
    int? OwnerId,
    bool? IsPublic,
    string? Settings
);

public sealed record UpdateProjectRequest(
    string Name,
    string? Description,
    bool? IsPublic,
    string? Settings
);
