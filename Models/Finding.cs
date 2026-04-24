public class Finding
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public AuditSession Session { get; set; } = null!;
    public FindingCategory Category { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ClauseRef { get; set; } = string.Empty;
    public DateTime FoundAt { get; set; } = DateTime.UtcNow;
    public FindingStatus Status { get; set; } = FindingStatus.Open;
    public CAPA? Capa { get; set; }
}

public enum FindingCategory { MajorNC, MinorNC, Observation, OFI }
public enum FindingStatus { Open, InProgress, Closed }
