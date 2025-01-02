using Microsoft.AspNetCore.Identity;

namespace Identity.Server.Models;

public class ApplicationRole : IdentityRole
{
    public const string StandardUser = "StandardUser";
    public const string Administrator = "Administrator";
    public const string Distributor = "Distributor";

    /// <summary>
    /// A dictionary that stores the priority of each role.
    /// The lower the value, the higher the permission.
    /// </summary>
    public static readonly Dictionary<string, int> RolePermissionPriority = new()
    {
        { Administrator, 0},
        { Distributor, 1},
        { StandardUser, 2},
    };

    public static string GetHighestRole(IEnumerable<string> roles)
    {
        PriorityQueue<string, int> queue = new();
        foreach (var role in roles)
        {
            if (RolePermissionPriority.TryGetValue(role, out int value))
            {
                queue.Enqueue(role, value);
            }
        }
        return queue.Count > 0 ? queue.Peek() : StandardUser;
    }
}
