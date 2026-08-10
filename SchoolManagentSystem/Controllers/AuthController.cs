using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SchoolManagentSystem_API.DBContext;
using SchoolManagentSystem_API.Dtos.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SchoolManagentSystem_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context,
                              IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDTO request)
        {
            // Check user credentials
            var user = await _context.User
                .FirstOrDefaultAsync(x =>
                    x.Email == request.Email &&
                    x.Password == request.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Invalid email or password." });
            }

            // Get user's role
            var userRole = await _context.UserRoles
                .Include(x => x.Roles)
                .FirstOrDefaultAsync(x => x.UserID == user.UserID);

            if (userRole == null || userRole.Roles == null)
            {
                return BadRequest(new { message = "Role not assigned to this user." });
            }

            // Create Claims
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserID!.ToString() ?? string.Empty),
        new Claim(JwtRegisteredClaimNames.Email, user.Email!),
        new Claim(ClaimTypes.Name, user.UserName!),
        new Claim(ClaimTypes.Role, userRole.Roles.RoleName!),
        new Claim("UserID", user.UserID.ToString()?? string.Empty),
        new Claim("custom", "naveen testing")
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                Token = jwtToken,
                UserId = user.UserID,
                UserName = user.UserName,
                Email = user.Email,
                Role = userRole.Roles.RoleName
            });
        }
    }
}
