using DiagramFlow.API.Data;
using DiagramFlow.API.Endpoints;
using DiagramFlow.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IDiagramRepository, DiagramRepository>();
builder.Services.AddScoped<IDiagramHistoryRepository, DiagramHistoryRepository>();
builder.Services.AddScoped<ICollaboratorRepository, CollaboratorRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

var app = builder.Build();

app.UseCors("AllowAll");

app.MapRoleEndpoints();
app.MapUserEndpoints();
app.MapProjectEndpoints();
app.MapDiagramEndpoints();
app.MapDiagramHistoryEndpoints();
app.MapCollaboratorEndpoints();
app.MapTemplateEndpoints();
app.MapAuthEndpoints();

app.Run();

