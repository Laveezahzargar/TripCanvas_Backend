using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using P6_Travel_Planner_Backend.Data;
using P6_Travel_Planner_Backend.DTOs;
using P6_Travel_Planner_Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace P6_Travel_Planner_Backend.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;

        public UserController(AppDbContext context, IConfiguration configuration, ILogger<UserController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        // ✅ REGISTER
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            _logger.LogInformation("Registration attempt for email: {Email}", dto.Email);

            // Check if email exists
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            {
                _logger.LogWarning("Registration failed. Email already exists: {Email}", dto.Email);
                return BadRequest("Email already exists");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "User registered successfully. UserId: {UserId}, Email: {Email}",
                user.Id,
                user.Email);

            return Ok("User registered successfully");
        }

        // ✅ LOGIN
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                _logger.LogWarning(
                    "Login failed. User not found for email: {Email}",
                    dto.Email);
                return Unauthorized("Invalid email");
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning(
                    "Login failed. Invalid password for email: {Email}",
                    dto.Email);
                return Unauthorized("Invalid password");
            }

            var claims = new[]
             {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            _logger.LogInformation(
               "User logged in successfully. UserId: {UserId}, Email: {Email}",
               user.Id,
               user.Email);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                id = user.Id,
                email = user.Email,
                role = user.Role,
                username = user.Username
            });
        }

        // ✅ VIEW PROFILE
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogWarning("Profile access failed. Invalid user claim.");
                return Unauthorized();
            }

            _logger.LogInformation(
                "Fetching profile for UserId: {UserId}",
                userId);

            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Username,
                    u.Email,
                    u.Role
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning(
                    "Profile not found for UserId: {UserId}",
                    userId);

                return NotFound("User not found");
            }

            _logger.LogInformation(
                "Profile retrieved successfully for UserId: {UserId}",
                userId);

            return Ok(user);
        }
    }
}
