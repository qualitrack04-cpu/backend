namespace QualiTrack.DTOs;

public class KpiDto
{
    public int TotalCapaAssigned { get; set; }
    public int TotalCapaClosed { get; set; }
    public int TotalCapaOpenInProgress { get; set; }
    public int TotalCapaClosedOnTime { get; set; }
    public int TotalFindingsReported { get; set; }
    public double OnTimeCompletionRate { get; set; } // 0.0 - 1.0
}