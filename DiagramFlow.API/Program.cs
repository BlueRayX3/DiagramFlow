using Microsoft.Data.SqlClient;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

// Güvenlik (CORS) ayarını tek ve temiz bir hale getirdik
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowReact");

// appsettings.json'dan veritabanı bağlantı adresini alıyoruz.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// TÜM PROJELERİ GETİR
app.MapGet("/api/projects", async () =>
{
    using var connection = new SqlConnection(connectionString);

    var sql = @"
        SELECT 
            p.ProjectID, 
            p.Name AS ProjectName, 
            p.Description,
            p.OwnerID, -- React'in projeyi kimin sildiğini anlaması için OwnerID'yi de gönderiyoruz
            u.Username AS OwnerName
        FROM Projects p
        INNER JOIN Users u ON p.OwnerID = u.UserID";

    var projects = await connection.QueryAsync(sql);
    return Results.Ok(projects);
});

// BELİRLİ KULLANICI PROJELERİNİ GETİR
app.MapGet("/api/user-projects/{userId}", async (int userId) =>
{
    string connString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    using var connection = new SqlConnection(connString);

    var sql = @"
        SELECT 
            u.Username,
            p.Name AS ProjeAdi, 
            p.CreatedAt AS OlusturulmaTarihi
        FROM Projects p
        INNER JOIN Users u ON p.OwnerID = u.UserID
        WHERE u.UserID = @Id";

    var userProjects = await connection.QueryAsync(sql, new { Id = userId });
    return Results.Ok(userProjects);
});

// 🚀 İŞTE DÜZELTTİĞİMİZ YER: YENİ PROJE EKLEME (POST)
app.MapPost("/api/projects", async (NewProject project) =>
{
    string connString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    using var connection = new SqlConnection(connString);

    // "1" olan yeri "@OwnerID" olarak değiştirdik! Artık kim giriş yaptıysa onun ID'si yazılacak.
    var sql = @"INSERT INTO Projects (Name, Description, OwnerID, IsPublic, CreatedAt) 
                VALUES (@Name, @Description, @OwnerID, 1, GETDATE())";

    // Gelen modelin içindeki OwnerID'yi SQL'e bağlıyoruz
    await connection.ExecuteAsync(sql, project);
    return Results.Ok();
});

// PROJE SİLME (DELETE)
app.MapDelete("/api/projects/{id}", async (int id) =>
{
    try
    {
        string connString = builder.Configuration.GetConnectionString("DefaultConnection")!;
        using var connection = new SqlConnection(connString);
        var sql = "DELETE FROM Projects WHERE ProjectID = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
        return Results.Ok();
    }
    catch
    {
        return Results.BadRequest("Bu proje silinemez (İçinde bağlı veriler olabilir).");
    }
});

app.MapControllers();
app.Run(); // Bu hep en sonda kalmalı!

// 🚀 ŞABLONU DA GÜNCELLEDİK: Artık C# React'ten gelen OwnerID verisini de içeri alabiliyor
public record NewProject(string Name, string Description, int OwnerID);