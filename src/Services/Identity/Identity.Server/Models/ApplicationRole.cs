using Microsoft.AspNetCore.Identity;

namespace Identity.Server.Models;

public class ApplicationRole : IdentityRole<string>
{
    /// <summary>
    /// Smaller values have higher permissions.
    /// </summary>
    public int Priority { get; set; }
    
    // Predefined roles
    // TODO: may be better to use a random GUID in production?
    
    public static readonly ApplicationRole StandardUser = new()
    {
        Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2).ToString(),
        Name = "StandardUser",
        Priority = 2,
        NormalizedName = "STANDARD_USER"
    };
    
    public static readonly ApplicationRole Distributor = new()
    {
        Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1).ToString(),
        Name = "Distributor",
        Priority = 1,
        NormalizedName = "DISTRIBUTOR"
    };
    
    public static readonly ApplicationRole Administrator = new()
    {
        Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0).ToString(),
        Name = "Administrator",
        Priority = 0,
        NormalizedName = "ADMINISTRATOR"
    };
    
    public static readonly Dictionary<string, ApplicationRole> RolePermissionPriority = new()
    {
        {StandardUser.Name!, StandardUser},
        {Distributor.Name!, Distributor},
        {Administrator.Name!, Administrator}
    };

    /// <summary>
    /// Get the smallest (highest permission) role from the roles.
    /// </summary>
    public static ApplicationRole GetHighestRole(IEnumerable<string> roles)
    {
        ApplicationRole result = StandardUser;
        
        foreach (var role in roles)
        {
            if (RolePermissionPriority.TryGetValue(role, out ApplicationRole? value))
            {
                if (value.Priority < result.Priority)
                {
                    result = value;
                }       
            }
        }
        
        return result;
    }
    
    public static IQueryable<ApplicationRole> GetAllRoles()
    {
        return RolePermissionPriority.Values.AsQueryable();
    }
}