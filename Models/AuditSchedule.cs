public class AuditSchedule
{
    public Guid Id { get; set; }
    public Guid AuditPlanId { get; set; }
    public AuditPlan AuditPlan { get; set; } = null!;
    public string ClauseRef { get; set; } = string.Empty;
    public Guid AuditorId { get; set; }
    public User Auditor { get; set; } = null!;
    public DateOnly ScheduledDate { get; set; }
    public string Department { get; set; } = string.Empty;
}
