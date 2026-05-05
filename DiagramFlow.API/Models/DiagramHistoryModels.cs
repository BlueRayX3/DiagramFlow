namespace DiagramFlow.API.Models;

public sealed record DiagramHistory(
    int HistoryId,
    int DiagramId,
    int ChangedBy,
    string PreviousContent,
    string? ChangeDescription,
    DateTime CreatedAt
);

public sealed record CreateDiagramHistoryRequest(
    int DiagramId,
    int ChangedBy,
    string PreviousContent,
    string? ChangeDescription
);
