using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DiagramFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Kendi veritabanı bağlantı cümleni buraya yaz
        private readonly string _connectionString = "Server=.\\SQLEXPRESS;Database=DiagramFlowDB;Trusted_Connection=True;";

        // GİRİŞ YAPMA (LOGIN) UCU
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT UserID, Username, FirstName, LastName FROM Users WHERE Username = @Username AND PasswordHash = @Password";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", request.Username);
                    // Not: Ödev projesi olduğu için şifreyi direkt karşılaştırıyoruz. 
                    // Gerçek hayatta burada Hash kontrolü yapılır.
                    cmd.Parameters.AddWithValue("@Password", request.Password);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var user = new
                            {
                                UserID = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                FirstName = reader.GetString(2),
                                LastName = reader.GetString(3)
                            };
                            return Ok(user); // Kullanıcı bulundu, bilgileri React'e gönder
                        }
                    }
                }
            }
            return Unauthorized("Kullanıcı adı veya şifre hatalı!");
        }

        // KAYIT OLMA (REGISTER) UCU
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Kullanıcı adının veya emailin aynısı var mı diye kontrol edebilirsin, şimdilik direkt ekliyoruz.
                // RoleID = 2 (Editor yetkisi) olarak atanıyor.
                string query = @"
                    INSERT INTO Users (Username, Email, PasswordHash, RoleID, FirstName, LastName, IsActive, CreatedAt) 
                    VALUES (@Username, @Email, @Password, 2, @FirstName, @LastName, 1, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", request.Username);
                    cmd.Parameters.AddWithValue("@Email", request.Email);
                    cmd.Parameters.AddWithValue("@Password", request.Password); // Şifre hash'lenmeden kaydediliyor (ödev için)
                    cmd.Parameters.AddWithValue("@FirstName", request.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", request.LastName);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        return Ok(new { Message = "Kayıt başarılı! Şimdi giriş yapabilirsiniz." });
                    }
                    catch (SqlException ex)
                    {
                        return BadRequest("Bu kullanıcı adı veya e-posta zaten kullanılıyor olabilir.");
                    }
                }
            }
        }
    }

    // API'nin dışarıdan beklediği veri modelleri
    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest : LoginRequest
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}