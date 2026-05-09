using QualiTrack.Models;

namespace QualiTrack.Models;

public class Checklist
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Standard { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ChecklistItem> Items { get; set; } = [];

}