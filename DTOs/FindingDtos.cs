using System.ComponentModel.DataAnnotations;
using QualiTrack.Models;

namespace QualiTrack.DTOs;

public class CreateFindingRequest
{
    public Guid? SessionId { get; set; }

    [Required(ErrorMessage = "Category wajib diisi")]
    public FindingCategory? Category { get; set; }

    [Required(ErrorMessage = "Description wajib diisi")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description harus antara 10 sampai 2000 karakter")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "ClauseRef wajib diisi")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "ClauseRef harus antara 1 sampai 200 karakter")]
    public string ClauseRef { get; set; } = string.Empty;
}
