namespace DiagramFlow.API.Models;

public sealed record Role(
    int RoleId,
    string RoleName,
    string? Description,
    bool CanCreateProject,
    bool CanDeleteProject,
    bool CanManageUsers
);

public sealed record CreateRoleRequest(
    string RoleName,
    string? Description,
    bool CanCreateProject,
    bool CanDeleteProject,
    bool CanManageUsers
);

public sealed record UpdateRoleRequest(
    string RoleName,
    string? Description,
    bool CanCreateProject,
    bool CanDeleteProject,
    bool CanManageUsers
);
