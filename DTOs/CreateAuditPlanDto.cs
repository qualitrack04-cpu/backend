using System.ComponentModel.DataAnnotations;

namespace QualiTrack.DTOs;

public class CreateAuditPlanDto
{
    [Required(ErrorMessage = "Title wajib diisi")]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;
    
    [Range(2020, 2030)]
    public int Year { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Standard { get; set; } = string.Empty;
    
    public List<CreateScheduleDto>? Schedules { get; set; }
}

public class CreateScheduleDto
{
    [Required]
    public string ClauseRef { get; set; } = string.Empty;
    
    [Required]
    public Guid AuditorId { get; set; }
    
    public DateOnly ScheduledDate { get; set; }
    
    public string Department { get; set; } = string.Empty;
}