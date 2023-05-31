using Lagoon.Shared;
using System.Collections;

namespace Lagoon.Helpers;

/// <summary>
/// Helper to configure roles from policy names.
/// </summary>
/// <typeparam name="TAppRoles">Roles enumeration.</typeparam>
/// <typeparam name="TAppPolicies">The application policies definition.</typeparam>
public class RequireRolePolicies<TAppRoles, TAppPolicies> : IEnumerable<KeyValuePair<string, List<string>>>
    where TAppRoles : struct, Enum
    where TAppPolicies : Policies
{

    #region fields

    private Dictionary<string, List<string>> _policies;

    #endregion

    #region initialisation

    /// <summary>
    /// New instance.
    /// </summary>
    public RequireRolePolicies()
    {
        _policies = new();
        // Register all known policies of the parent type TAppPolicies in case some policy names are not assigned to any role.
        // (else, if a policy is not assigned to a role, the check of the policy will crash the application
        // with a System.InvalidOperationException: The AuthorizationPolicy named: '{policy}' was not found).
        // The application's "Policies" type can contains constants to defined policies that are not "RequireRole", so we seek from the base type.
        IEnumerable<string> appPolicies = typeof(TAppPolicies).BaseType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            // Ignore duplicates with the redefinition try of constants with "new const"
            .Reverse() // To have the ancestor const definition first
            .GroupBy(c=>c.Name)
            .ToDictionary(g=>g.Key,g=>g.First())
            // Select only the constant value
            .Select(p => (string)p.Value.GetRawConstantValue());
        foreach (string policy in appPolicies)
        {
            _policies.Add(policy, null);
        }
    }

    #endregion

    #region methods Add (Compose a role)

    /// <summary>
    /// Declare a role without any policy.
    /// </summary>
    /// <param name="_">The role with no policies.</param>
    public void Add(TAppRoles _)
    {
        // To avoid CA1825 warning (Avoid zero-length array allocations)
    }

    /// <summary>
    /// Add a role by copying policies from another role and optionally add custom policies.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <param name="policies">A set of policy name to add.</param>
    public void Add(TAppRoles role, params string[] policies)
    {
        string roleName = role.ToString();
        foreach (string policy in policies)
        {
            List<string> roles = GetRoleList(policy);
            if (!roles.Contains(roleName))
            {
                roles.Add(roleName);
            }
        }
    }

    /// <summary>
    /// Add a role by copying policies from another role and optionally add custom policies.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <param name="fromRole">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="policies">A set of policy name to add.</param>
    public void Add(TAppRoles role, TAppRoles fromRole, params string[] policies)
    {
        CopyPolicies(role, fromRole);
        Add(role, policies);
    }

    /// <summary>
    /// Add a role by copying policies from another role and optionally add custom policies.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <param name="fromRole1">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole2">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="policies">A set of policy name to add.</param>
    public void Add(TAppRoles role, TAppRoles fromRole1, TAppRoles fromRole2, params string[] policies)
    {
        CopyPolicies(role, fromRole1, fromRole2);
        Add(role, policies);
    }

    /// <summary>
    /// Add a role by copying policies from another role and optionally add custom policies.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <param name="fromRole1">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole2">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole3">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="policies">A set of policy name to add.</param>
    public void Add(TAppRoles role, TAppRoles fromRole1, TAppRoles fromRole2, TAppRoles fromRole3, params string[] policies)
    {
        CopyPolicies(role, fromRole1, fromRole2, fromRole3);
        Add(role, policies);
    }

    /// <summary>
    /// Add a role by copying policies from another role and optionally add custom policies.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <param name="fromRole1">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole2">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole3">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole4">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="policies">A set of policy name to add.</param>
    public void Add(TAppRoles role, TAppRoles fromRole1, TAppRoles fromRole2, TAppRoles fromRole3, TAppRoles fromRole4, params string[] policies)
    {
        CopyPolicies(role, fromRole1, fromRole2, fromRole3, fromRole4);
        Add(role, policies);
    }

    /// <summary>
    /// Add a role by copying policies from another role and optionally add custom policies.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <param name="fromRole1">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole2">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole3">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole4">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole5">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="policies">A set of policy name to add.</param>
    public void Add(TAppRoles role, TAppRoles fromRole1, TAppRoles fromRole2, TAppRoles fromRole3, TAppRoles fromRole4, TAppRoles fromRole5, params string[] policies)
    {
        CopyPolicies(role, fromRole1, fromRole2, fromRole3, fromRole4, fromRole5);
        Add(role, policies);
    }

    /// <summary>
    /// Add a role by copying policies from another role and optionally add custom policies.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <param name="fromRole1">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole2">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole3">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole4">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole5">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole6">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="policies">A set of policy name to add.</param>
    public void Add(TAppRoles role, TAppRoles fromRole1, TAppRoles fromRole2, TAppRoles fromRole3, TAppRoles fromRole4, TAppRoles fromRole5, TAppRoles fromRole6, params string[] policies)
    {
        CopyPolicies(role, fromRole1, fromRole2, fromRole3, fromRole4, fromRole5, fromRole6);
        Add(role, policies);
    }

    /// <summary>
    /// Add a role by copying policies from another role and optionally add custom policies.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <param name="fromRole1">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole2">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole3">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole4">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole5">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole6">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole7">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="policies">A set of policy name to add.</param>
    public void Add(TAppRoles role, TAppRoles fromRole1, TAppRoles fromRole2, TAppRoles fromRole3, TAppRoles fromRole4, TAppRoles fromRole5, TAppRoles fromRole6, TAppRoles fromRole7, params string[] policies)
    {
        CopyPolicies(role, fromRole1, fromRole2, fromRole3, fromRole4, fromRole5, fromRole6, fromRole7);
        Add(role, policies);
    }

    /// <summary>
    /// Add a role by copying policies from another role and optionally add custom policies.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <param name="fromRole1">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole2">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole3">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole4">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole5">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole6">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole7">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole8">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="policies">A set of policy name to add.</param>
    public void Add(TAppRoles role, TAppRoles fromRole1, TAppRoles fromRole2, TAppRoles fromRole3, TAppRoles fromRole4, TAppRoles fromRole5, TAppRoles fromRole6, TAppRoles fromRole7, TAppRoles fromRole8, params string[] policies)
    {
        CopyPolicies(role, fromRole1, fromRole2, fromRole3, fromRole4, fromRole5, fromRole6, fromRole7, fromRole8);
        Add(role, policies);
    }

    /// <summary>
    /// Add a role by copying policies from another role and optionally add custom policies.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <param name="fromRole1">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole2">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole3">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole4">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole5">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole6">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole7">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole8">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="fromRole9">A role, ALREADY ADDED, to copy policies from.</param>
    /// <param name="policies">A set of policy name to add.</param>
    public void Add(TAppRoles role, TAppRoles fromRole1, TAppRoles fromRole2, TAppRoles fromRole3, TAppRoles fromRole4, TAppRoles fromRole5, TAppRoles fromRole6, TAppRoles fromRole7, TAppRoles fromRole8, TAppRoles fromRole9, params string[] policies)
    {
        CopyPolicies(role, fromRole1, fromRole2, fromRole3, fromRole4, fromRole5, fromRole6, fromRole7, fromRole8, fromRole9);
        Add(role, policies);
    }

    private void CopyPolicies(TAppRoles toRole, params TAppRoles[] fromRoles)
    {
        foreach (TAppRoles role in fromRoles)
        {
            CopyPolicies(toRole, role);
        }
    }

    private void CopyPolicies(TAppRoles toRole, TAppRoles fromRole)
    {
        string roleName = toRole.ToString();
        string roleFromName = fromRole.ToString();
        foreach (List<string> roles in _policies.Values)
        {
            if (roles is not null && roles.Contains(roleFromName) && !roles.Contains(roleName))
            {
                roles.Add(roleName);
            }
        }
    }

    #endregion

    #region methods Add (Compose a policy)

    /// <summary>
    /// Declare a policy assigne to no roles.
    /// </summary>
    /// <param name="_">The policy assigned with no roles.</param>
    public void Add(string _)
    {
        // To avoid CA1825 warning (Avoid zero-length array allocations)
    }

    /// <summary>
    /// Add a role by copying policies from another role and optionally add custom policies.
    /// </summary>
    /// <param name="policy">The policy to add.</param>
    /// <param name="roles">A set of roles to add.</param>
    public void Add(string policy, params TAppRoles[] roles)
    {
        List<string> list = GetRoleList(policy);
        foreach (TAppRoles role in roles)
        {
            string roleName = role.ToString();
            if (!list.Contains(roleName))
            {
                list.Add(roleName);
            }
        }
    }

    #endregion

    #region methods

    /// <summary>
    /// Get the initialized role list for a policy name.
    /// </summary>
    /// <param name="policy">The policy name.</param>
    /// <returns>The initialized role list for a policy name.</returns>
    private List<string> GetRoleList(string policy)
    {
        if (!_policies.TryGetValue(policy, out List<string> roles))
        {
            roles = new();
            _policies.Add(policy, roles);
        }
        else
        {
            roles ??= new();
            _policies[policy] = roles;
        }
        return roles;
    }

    #endregion

    #region methods IEnumerator implementation

    /// <summary>
    /// Returns an enumerator that iterates through the System.Collections.Generic.Dictionary.
    /// </summary>
    /// <returns>An enumerator that iterates through the System.Collections.Generic.Dictionary.</returns>
    public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
    {
        return _policies.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the System.Collections.Generic.Dictionary.
    /// </summary>
    /// <returns>An enumerator that iterates through the System.Collections.Generic.Dictionary.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _policies.GetEnumerator();
    }

    #endregion

}
