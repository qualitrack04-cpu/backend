using QualiTrack.Models;
namespace QualiTrack.Models;

public class EvidenceFile
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty; // Supabase Storage path
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public Guid? FindingId { get; set; } 
    // Polymorphic — salah satu dari dua ini diisi
    public Guid? AuditResponseId { get; set; }
    public Guid? CapaActionId { get; set; }
}