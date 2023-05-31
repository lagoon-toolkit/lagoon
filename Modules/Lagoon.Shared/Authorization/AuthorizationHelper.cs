namespace Lagoon.Shared.Authorization;

/// <summary>
/// Authorization helper methods.
/// </summary>
public static class AuthorizationHelper
{

    /// <summary>
    /// Register polices from a dictionnary definition.
    /// </summary>
    /// <param name="options">The authorization options.</param>
    /// <param name="policies">The policies to register.</param>
    public static void AddRequireRolePolicies(this AuthorizationOptions options, IEnumerable<KeyValuePair<string, List<string>>> policies)
    {
        foreach (KeyValuePair<string, List<string>> policy in policies)
        {
            // "[-_-]" is a fake role, an invalid name for role enum : it's use when a policy is not associated to a role to avoid error
            options.AddPolicy(policy.Key, p => p.RequireRole(policy.Value ?? new() { "[-_-]" }));
        }
    }

}
