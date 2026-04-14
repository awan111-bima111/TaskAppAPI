using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskAppAPI.Data;
using TaskAppAPI.Models;
using BCrypt.Net;
using System.ComponentModel.DataAnnotations;

namespace TaskAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request kosong" });

                if (string.IsNullOrWhiteSpace(request.Name) ||
                    string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Name, email, dan password wajib diisi" });
                }

                if (_context.Users.Any(x => x.Email == request.Email))
                    return BadRequest(new { message = "Email sudah terdaftar" });

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                var token = GenerateToken(user);

                return Ok(new
                {
                    message = "Register berhasil",
                    token = token,
                    user = new
                    {
                        user.Id,
                        user.Name,
                        user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Terjadi error saat register",
                    error = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { message = "Request kosong" });

                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Email dan password wajib diisi" });
                }

                var user = _context.Users.FirstOrDefault(x => x.Email == request.Email);

                if (user == null)
                    return Unauthorized(new { message = "Email tidak ditemukan" });

                if (string.IsNullOrWhiteSpace(user.PasswordHash))
                    return Unauthorized(new { message = "Data password tidak valid, register ulang" });

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return Unauthorized(new { message = "Password salah" });

                var token = GenerateToken(user);

                return Ok(new
                {
                    message = "Login berhasil",
                    token = token,
                    user = new
                    {
                        user.Id,
                        user.Name,
                        user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Terjadi error saat login",
                    error = ex.Message
                });
            }
        }

        private string GenerateToken(User user)
        {
            var key = _config["Jwt:Key"] ?? "TaskAppAPI_SuperSecret_Key_2026_123456";
            var keyBytes = Encoding.UTF8.GetBytes(key);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("email", user.Email),
                    new Claim("name", user.Name)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class RegisterDto
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }

    public class LoginDto
    {
        [Required]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
}