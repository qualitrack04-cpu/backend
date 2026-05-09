using System.ComponentModel.DataAnnotations;

namespace QualiTrack.DTOs;

public class CreateCapaRequest
{
    [Required(ErrorMessage = "RootCause wajib diisi")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "RootCause harus antara 5 sampai 1000 karakter")]
    public string RootCause { get; set; } = string.Empty;

    [Required(ErrorMessage = "CorrectiveAction wajib diisi")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "CorrectiveAction harus antara 5 sampai 1000 karakter")]
    public string CorrectiveAction { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "PreventiveAction maksimal 1000 karakter")]
    public string? PreventiveAction { get; set; }

    [Required(ErrorMessage = "Deadline wajib diisi")]
    public DateOnly? Deadline { get; set; }

    [Required(ErrorMessage = "PIC harus diisi")]
    public Guid? PicId { get; set; }
}

public class UpdateCapaRequestDto
{
    [Required(ErrorMessage = "RootCause wajib diisi")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "RootCause harus antara 5 sampai 1000 karakter")]
    public string RootCause { get; set; } = string.Empty;

    [Required(ErrorMessage = "CorrectiveAction wajib diisi")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "CorrectiveAction harus antara 5 sampai 1000 karakter")]
    public string CorrectiveAction { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "PreventiveAction maksimal 1000 karakter")]
    public string? PreventiveAction { get; set; }

    [Required(ErrorMessage = "Deadline wajib diisi")]
    public DateOnly? Deadline { get; set; }

    public Guid? PicId { get; set; }
}

public class AddCapaActionRequest
{
    [Required(ErrorMessage = "Description wajib diisi")]
    [StringLength(1000, MinimumLength = 5, ErrorMessage = "Description harus antara 5 sampai 1000 karakter")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "DoneById wajib diisi")]
    public Guid? DoneById { get; set; }
}

public class CloseOutVerificationRequest
{
    [Required(ErrorMessage = "IsEffective wajib diisi")]
    public bool? IsEffective { get; set; }

    [Required(ErrorMessage = "VerificationNotes wajib diisi")]
    [StringLength(2000, MinimumLength = 5, ErrorMessage = "VerificationNotes harus antara 5 sampai 2000 karakter")]
    public string VerificationNotes { get; set; } = string.Empty;

    [Required(ErrorMessage = "VerifiedById wajib diisi")]
    public Guid? VerifiedById { get; set; }
}
