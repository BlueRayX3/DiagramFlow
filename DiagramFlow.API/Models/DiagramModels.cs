namespace DiagramFlow.API.Models;

public sealed record Diagram(
    int DiagramId,
    int ProjectId,
    int CreatedBy,
    string Title,
    string DiagramType,
    string Content,
    int Version,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public sealed record CreateDiagramRequest(
    int ProjectId,
    int CreatedBy,
    string Title,
    string DiagramType,
    string Content
);

public sealed record UpdateDiagramRequest(
    string Title,
    string DiagramType,
    string Content,
    int Version
);
