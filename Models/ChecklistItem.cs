// Models/ChecklistItem.cs

public class ChecklistItem
{
     public Guid Id { get; set; }
    public Guid ChecklistId { get; set; }
    public Checklist Checklist { get; set; } = null!;
    public string Question { get; set; } = string.Empty;
    public string ClauseRef { get; set; } = string.Empty;
    public int OrderIndex { get; set; }
}