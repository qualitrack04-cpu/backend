using QualiTrack.Models;

namespace QualiTrack.Models;

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string QualityManager = "QualityManager";
    public const string AuditorInternal = "AuditorInternal";
    public const string Auditee = "Auditee";
    
    public static readonly string[] AllRoles = { Admin, QualityManager, AuditorInternal, Auditee };
    
    public static bool IsValidRole(string role)
    {
        return AllRoles.Contains(role);
    }
}