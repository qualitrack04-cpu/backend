using System.ComponentModel.DataAnnotations;  // ← TAMBAHKAN INI!

namespace QualiTrack.Models;

public class User
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Nama lengkap wajib diisi")]
    public string FullName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Email wajib diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    public string Email { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Password wajib diisi")]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Role wajib diisi")]
    public string Role { get; set; } = "Auditee";
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}