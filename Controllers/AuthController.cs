using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using QualiTrack.Data;
using QualiTrack.DTOs;
using QualiTrack.Models;
using QualiTrack.Filters;

namespace QualiTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
[ValidateModelAttribute]
public class AuthController(AppDbContext db, IConfiguration config) : ControllerBase
{
    private static readonly string[] ValidRoles = ["QualityManager", "Auditor", "Auditee", "Admin"];

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        if (!ValidRoles.Contains(req.Role))
            return BadRequest("Role tidak valid. Pilih: QualityManager, Auditor, Auditee, Admin");

        if (req.Password.Length < 6)
            return BadRequest("Password minimal 6 karakter");

        if (await db.Users.AnyAsync(u => u.Email == req.Email))
            return BadRequest("Email sudah terdaftar");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = req.FullName,
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = req.Role,
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Ok(new { message = "Registrasi berhasil", userId = user.Id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized("Email atau password salah");

        var token = GenerateJwt(user);
        return Ok(new AuthResponse(token, user.Role, user.FullName));
    }

    [HttpGet("whoami")]
    [Authorize]
    public IActionResult WhoAmI()
    {
        return Ok(new
        {
            isAuthenticated = User.Identity != null && User.Identity.IsAuthenticated,
            name = User.Identity != null ? User.Identity.Name : null,
            claims = User.Claims.Select(c => new { c.Type, c.Value })
        });
    }

    [HttpGet("auditors")]
    [Authorize(Roles = "Admin,QualityManager")]
    public async Task<IActionResult> GetAuditors()
    {
        var auditors = await db.Users
            .Where(u => u.Role == "Auditor" || u.Role == "QualityManager")
            .Select(u => new { u.Id, u.FullName, u.Role })
            .ToListAsync();

        return Ok(new { message = "Daftar auditor berhasil diambil", total = auditors.Count, data = auditors });
    }

    [HttpGet("users")]
    [Authorize(Roles = "Admin,QualityManager")]
    public async Task<IActionResult> GetUsers([FromQuery] string? role)
    {
        var query = db.Users.AsQueryable();
        
        if (!string.IsNullOrEmpty(role))
            query = query.Where(u => u.Role == role);

        var users = await query
            .Select(u => new { u.Id, u.FullName, u.Email, u.Role, u.Status })
            .ToListAsync();

        return Ok(new { message = "Daftar user berhasil diambil", total = users.Count, data = users });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout berhasil" });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest req)
    {
        if (req.NewPassword.Length < 6)
            return BadRequest(new { message = "Password minimal 6 karakter" });

        if (string.Equals(req.NewPassword, req.ConfirmPassword) == false)
            return BadRequest(new { message = "Password dan konfirmasi password tidak sama" });

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user is null)
            return NotFound(new { message = "Email tidak ditemukan" });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        await db.SaveChangesAsync();

        return Ok(new { message = "Password berhasil direset" });
    }

    private string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
