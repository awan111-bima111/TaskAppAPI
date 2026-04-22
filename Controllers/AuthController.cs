using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskAppAPI.Data;
using TaskAppAPI.Models;
using System.Linq;

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

        // ================= REGISTER =================
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("data kosong");

            var email = req.Email.Trim().ToLower();
            var password = req.Password.Trim();
            var name = req.Name?.Trim() ?? "";

            Console.WriteLine($"REGISTER -> {email}");

            var exist = _context.Users.FirstOrDefault(x => x.Email.ToLower().Trim() == email);
            if (exist != null)
                return BadRequest("email sudah dipakai");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "register sukses" });
        }

        // ================= LOGIN =================
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto req)
        {
            Console.WriteLine("===== LOGIN HIT =====");

            if (req == null)
            {
                Console.WriteLine("REQ NULL");
                return BadRequest("request kosong");
            }

            Console.WriteLine("EMAIL RAW: " + req.Email);
            Console.WriteLine("PASSWORD RAW: " + req.Password);

            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("data kosong");

            var email = req.Email.Trim().ToLower();
            var password = req.Password.Trim();

            Console.WriteLine("EMAIL FIX: " + email);
            Console.WriteLine("PASSWORD FIX: " + password);

            var user = _context.Users
                .FirstOrDefault(x => x.Email.ToLower().Trim() == email);

            if (user == null)
            {
                Console.WriteLine("USER TIDAK DITEMUKAN");
                return Unauthorized("email tidak ditemukan");
            }

            Console.WriteLine("USER DITEMUKAN: " + user.Email);

            var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            Console.WriteLine("PASSWORD MATCH: " + isValid);

            if (!isValid)
                return Unauthorized("password salah");

            var key = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
                return StatusCode(500, "JWT key tidak ada");

            var keyBytes = Encoding.UTF8.GetBytes(key);

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("email", user.Email),
                    new Claim("name", user.Name ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256)
            });

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email
                }
            });
        }
    }
}