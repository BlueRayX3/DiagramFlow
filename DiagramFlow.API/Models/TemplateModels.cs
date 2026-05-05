namespace DiagramFlow.API.Models;

public sealed record DiagramTemplate(
    int TemplateId,
    string Name,
    string? Description,
    string DiagramType,
    string Content,
    bool IsSystemTemplate,
    int? CreatedBy,
    DateTime CreatedAt
);

public sealed record CreateTemplateRequest(
    string Name,
    string? Description,
    string DiagramType,
    string Content,
    bool IsSystemTemplate,
    int? CreatedBy
);

public sealed record UpdateTemplateRequest(
    string Name,
    string? Description,
    string DiagramType,
    string Content,
    bool IsSystemTemplate
);
