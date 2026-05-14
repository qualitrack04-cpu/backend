using System.ComponentModel.DataAnnotations;
using QualiTrack.Models;

namespace QualiTrack.DTOs;

public class CreateFindingRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public Guid? SessionId { get; set; }
    [Required]
    public FindingCategory? Category { get; set; }
    [Required]
    public string Description { get; set; } = string.Empty;
    public string ClauseRef { get; set; } = string.Empty;
}

public class UpdateFindingRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public FindingCategory? Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ClauseRef { get; set; } = string.Empty;
}
