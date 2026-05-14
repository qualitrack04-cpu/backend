using System.ComponentModel.DataAnnotations;

namespace QualiTrack.DTOs;

public class CreateAuditSessionDto
{
    [Required]
    public Guid ScheduleId { get; set; }
    
    [Required]
    public Guid ChecklistId { get; set; }
    
    public string? Notes { get; set; }
}

public class AuditSessionResponseDto
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid ChecklistId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
}