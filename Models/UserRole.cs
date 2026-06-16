using QualiTrack.Models;

namespace QualiTrack.Models;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string QualityManager = "QualityManager";
    public const string Auditor = "Auditor";
    public const string AuditorInternal = "AuditorInternal";
    public const string Auditee = "Auditee";
    
    public static readonly string[] AllRoles = { Admin, QualityManager, AuditorInternal, Auditee };
    
    public static bool IsValidRole(string role)
    {
        return role == Auditor || AllRoles.Contains(role);
    }

    public static string NormalizeRole(string role)
    {
        return role == Auditor ? AuditorInternal : role;
    }

    public static string GetClaimRole(string role)
    {
        return role == AuditorInternal ? Auditor : role;
    }
}