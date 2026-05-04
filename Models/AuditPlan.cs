using QualiTrack.Models;

namespace QualiTrack.Models;

public class AuditPlan
{
    public Guid Id { get; set; }
    public String Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Standard { get; set; } = string.Empty; // ISO9001, ISO14001, GMP
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

    public ICollection<AuditSchedule> Schedules { get; set; } = [];
}