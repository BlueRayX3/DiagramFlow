namespace DiagramFlow.API.Models;

public sealed record Collaborator(
    int CollaboratorId,
    int ProjectId,
    int UserId,
    string Permission,
    DateTime InvitedAt,
    DateTime? AcceptedAt
);

public sealed record CreateCollaboratorRequest(
    int ProjectId,
    int UserId,
    string Permission
);

public sealed record UpdateCollaboratorRequest(
    string Permission,
    DateTime? AcceptedAt
);
