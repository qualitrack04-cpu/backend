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
using QualiTrack.Services;

namespace QualiTrack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, IConfiguration config, IEmailService emailService) : ControllerBase
{
    private static readonly string[] ValidRoles = ["QualityManager", "Auditor", "Auditee", "Admin"];

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req)
    {
        if (!ValidRoles.Contains(req.Role))
            return BadRequest(new { message = "Role tidak valid. Pilih: QualityManager, Auditor, Auditee, Admin" });

        if (req.Password.Length < 6)
            return BadRequest(new { message = "Password minimal 6 karakter" });

        if (await db.Users.AnyAsync(u => u.Email == req.Email))
            return BadRequest(new { message = "Email sudah terdaftar" });

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = req.FullName,
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = req.Role,
            // TODO akhir Sprint 2: ganti Status = "Pending" dan EmailVerified = false setelah email service siap
            Status = "Active",
            EmailVerified = true,
            OtpCode = null,
            OtpExpiry = null,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Ok(new {
            message = "Registrasi berhasil! Silakan login.",
            email = user.Email
        });
    }

    // TODO akhir Sprint 2: aktifkan kembali setelah email service siap
    // [HttpPost("verify-email")]
    // [HttpPost("resend-otp")]

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized(new { message = "Email atau password salah" });

        // TODO akhir Sprint 2: aktifkan kembali setelah email service siap
        // if (!user.EmailVerified)
        //     return Unauthorized(new { message = "Email belum diverifikasi. Cek inbox email kamu." });

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

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout berhasil" });
    }

    [HttpPost("forgot-password/request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpRequest req)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user is null)
            return NotFound(new { message = "Email tidak ditemukan" });

        var otp = new Random().Next(100000, 999999).ToString();
        user.OtpCode = BCrypt.Net.BCrypt.HashPassword(otp);
        user.OtpExpiry = DateTime.UtcNow.AddMinutes(5);
        await db.SaveChangesAsync();

        await emailService.SendOtpAsync(user.Email, otp);

        return Ok(new { message = "Kode OTP telah dikirim ke email kamu" });
    }

    [HttpPost("forgot-password/verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest req)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user is null)
            return NotFound(new { message = "Email tidak ditemukan" });

        if (user.OtpCode is null || user.OtpExpiry is null)
            return BadRequest(new { message = "OTP belum di-request" });

        if (DateTime.UtcNow > user.OtpExpiry)
            return BadRequest(new { message = "OTP sudah kadaluarsa, minta OTP baru" });

        if (!BCrypt.Net.BCrypt.Verify(req.Otp, user.OtpCode))
            return BadRequest(new { message = "OTP tidak valid" });

        var resetToken = Guid.NewGuid().ToString();
        user.OtpCode = resetToken;
        user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);
        await db.SaveChangesAsync();

        return Ok(new { message = "OTP valid", resetToken = resetToken });
    }

    [HttpPost("forgot-password/reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user is null)
            return NotFound(new { message = "Email tidak ditemukan" });

        if (user.OtpCode is null || user.OtpExpiry is null)
            return BadRequest(new { message = "Reset token tidak valid" });

        if (DateTime.UtcNow > user.OtpExpiry)
            return BadRequest(new { message = "Reset token sudah kadaluarsa" });

        if (user.OtpCode != req.ResetToken)
            return BadRequest(new { message = "Reset token tidak valid" });

        if (req.NewPassword.Length < 6)
            return BadRequest(new { message = "Password minimal 6 karakter" });

        if (string.Equals(req.NewPassword, req.ConfirmPassword) == false)
            return BadRequest(new { message = "Password dan konfirmasi password tidak sama" });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
        user.OtpCode = null;
        user.OtpExpiry = null;
        await db.SaveChangesAsync();

        return Ok(new { message = "Password berhasil direset, silakan login" });
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
