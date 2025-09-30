using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WhiskTakers.Infrastructure;
using WhiskTakers.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace WhiskTakers.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly WhiskTakersDbContext _context;

        public AuthController(IConfiguration configuration, WhiskTakersDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Email and password are required" });
            }

            // Check database first
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user != null && VerifyPassword(request.Password, user.Password))
            {
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

            // Fallback to demo users for existing functionality
            var demoUsers = new[]
            {
                new { Email = "admin@whisktakers.com", Password = "admin123", Name = "Admin", Role = "admin" },
                new { Email = "waiter1@whisktakers.com", Password = "waiter123", Name = "John", Role = "waiter" },
                new { Email = "waiter2@whisktakers.com", Password = "waiter123", Name = "Sydney", Role = "waiter" },
                new { Email = "chef@whisktakers.com", Password = "chef123", Name = "Chef Marco", Role = "chef" },
                new { Email = "client@whisktakers.com", Password = "client123", Name = "Guest", Role = "client" }
            };

            var demoUser = demoUsers.FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);
            if (demoUser == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var demoToken = GenerateJwtToken(demoUser.Email, demoUser.Name, demoUser.Role);
            return Ok(new
            {
                token = demoToken,
                user = new
                {
                    email = demoUser.Email,
                    name = demoUser.Name,
                    role = demoUser.Role
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Email) || 
                string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Role))
            {
                return BadRequest(new { message = "All fields are required" });
            }

            if (request.Role != "waiter" && request.Role != "chef")
            {
                return BadRequest(new { message = "Role must be either 'waiter' or 'chef'" });
            }

            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "User with this email already exists" });
            }

            // Create new user
            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                Password = HashPassword(request.Password),
                Role = request.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "User created successfully" });
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

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
