using System.ComponentModel.DataAnnotations;

public record RegisterRequest(
    [Required(ErrorMessage = "Nama Wajib diisi")]
    string FullName,

    [Required(ErrorMessage = "Email Wajib diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    string Email,
    [Required(ErrorMessage = "Password Wajib diisi")]
    string Password,

    [Required(ErrorMessage = "Role Wajib diisi")]
    string Role
);

public record LoginRequest(
    [Required(ErrorMessage = "Email Wajib diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    string Email,
    [Required(ErrorMessage = "Password Wajib diisi")]
    string Password
);

public record AuthResponse(
    string Token,
    string Role,
    string FullName
);

public record UpdateCapaRequest(
    string RootCause,
    string CorrectiveAction,
    string? PreventiveAction,
    DateOnly Deadline,
    Guid? PicId
);

public record ForgotPasswordRequest(
    string Email,
    string NewPassword,
    string ConfirmPassword
);