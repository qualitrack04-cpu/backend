using QualiTrack.Models;

namespace QualiTrack.Models;

public class AuditPlan
{
    public Guid Id { get; set; }
    public String Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Standard { get; set; } = string.Empty; // ISO9001, ISO14001, GMP
    public AuditPriority Priority { get; set; } = AuditPriority.Common;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<AuditSchedule> Schedules { get; set; } = [];
}

public enum AuditPriority { Common, Priority }