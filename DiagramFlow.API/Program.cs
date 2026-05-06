using Microsoft.Data.SqlClient;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

// React/Frontend arayüzümüz bağlanırken engellenmesin diye CORS (Güvenlik) izni veriyoruz.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
builder.Services.AddControllers();
var app = builder.Build();

app.UseCors("AllowAll");

// appsettings.json'dan veritabanı bağlantı adresini alıyoruz.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

// /api/projects adresine gidildiğinde SQL sorgusunu çalıştırıp veriyi JSON olarak dönecek endpoint
app.MapGet("/api/projects", async () =>
{
    using var connection = new SqlConnection(connectionString);

    // Daha önce test ettiğimiz JOIN sorgusunun aynısı
    var sql = @"
        SELECT 
            p.ProjectID, 
            p.Name AS ProjectName, 
            p.Description,
            u.Username AS OwnerName
        FROM Projects p
        INNER JOIN Users u ON p.OwnerID = u.UserID
        WHERE p.IsPublic = 1";

    var projects = await connection.QueryAsync(sql);
    return Results.Ok(projects);
});

// YENİ ENDPOINT: Belirli bir kullanıcının projelerini ve oluşturulma tarihlerini getirir
app.MapGet("/api/user-projects/{userId}", async (int userId) =>
{
    // appsettings.json'dan veritabanı bağlantı adresini tekrar alıyoruz.
    string connString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    using var connection = new SqlConnection(connString);

    // Kullanıcının oluşturduğu projelerin isimlerini ve tarihlerini çeken sorgu
    var sql = @"
        SELECT 
            u.Username,
            p.Name AS ProjeAdi, 
            p.CreatedAt AS OlusturulmaTarihi
        FROM Projects p
        INNER JOIN Users u ON p.OwnerID = u.UserID
        WHERE u.UserID = @Id";

    // Dapper, URL'den gelen {userId} değerini güvenli bir şekilde @Id parametresine bağlar.
    var userProjects = await connection.QueryAsync(sql, new { Id = userId });

    return Results.Ok(userProjects);
});
// YENİ PROJE EKLEME (POST)
app.MapPost("/api/projects", async (NewProject project) =>
{
    string connString = builder.Configuration.GetConnectionString("DefaultConnection")!;
    using var connection = new SqlConnection(connString);

    // Varsayılan olarak admin_user (OwnerID = 1) adına ve halka açık (IsPublic = 1) ekliyoruz
    var sql = @"INSERT INTO Projects (Name, Description, OwnerID, IsPublic, CreatedAt) 
                VALUES (@Name, @Description, 1, 1, GETDATE())";

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
        // Eğer projenin içinde diyagramlar varsa SQL Foreign Key hatası verir, uygulamanın çökmesini önlüyoruz.
        return Results.BadRequest("Bu proje silinemez (İçinde bağlı veriler olabilir).");
    }
});
app.MapControllers();
app.Run(); // Bu hep en sonda kalmalı!

// Sayfanın en altına şu modeli ekle (C#'a gelen verinin şablonu)
public record NewProject(string Name, string Description);

