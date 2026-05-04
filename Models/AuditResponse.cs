using QualiTrack.Models;

namespace QualiTrack.Models;

public class AuditResponse
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public AuditSession Session { get; set; } = null!;
    public Guid ChecklistItemId { get; set; }
    public ChecklistItem ChecklistItem { get; set; } = null!;
    public ResponseAnswer Answer { get; set; }
    public string? Notes { get; set; }
    public ICollection<EvidenceFile> Evidences { get; set; } = [];
}

public enum ResponseAnswer { Conform, NotConform, NotApplicable }
