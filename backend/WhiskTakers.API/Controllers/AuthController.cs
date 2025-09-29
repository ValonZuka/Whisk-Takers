using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WhiskTakers.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Simple validation - in production, check against database
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email and password are required" });
            }

            // Demo users - replace with database lookup
            var users = new[]
            {
                new { Email = "admin@whisktakers.com", Password = "admin123", Name = "Admin", Role = "admin" },
                new { Email = "waiter1@whisktakers.com", Password = "waiter123", Name = "John", Role = "waiter" },
                new { Email = "waiter2@whisktakers.com", Password = "waiter123", Name = "Sydney", Role = "waiter" },
                new { Email = "chef@whisktakers.com", Password = "chef123", Name = "Chef Marco", Role = "chef" },
                new { Email = "client@whisktakers.com", Password = "client123", Name = "Guest", Role = "client" }
            };

            var user = users.FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(user.Email, user.Name, user.Role);

            return Ok(new
            {
                token = token,
                user = new
                {
                    email = user.Email,
                    name = user.Name,
                    role = user.Role
                }
            });
        }

        private string GenerateJwtToken(string email, string name, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role),
                new Claim("role", role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "WhiskTakers",
                audience: _configuration["Jwt:Audience"] ?? "WhiskTakers",
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
