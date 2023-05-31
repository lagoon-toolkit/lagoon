using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Lagoon.UI.Components;


/// <summary>
/// Service that can be used to check if a user belongs to a policy
/// </summary>
/// <remarks>
/// This Class 
///     - is added by default as a Scoped Services (and can be accessed via [Inject])
///     - should be used by components implementing the interface <see cref="ILgComponentPolicies" />
///     - LgComponentBase expose the "IsInPolicyAsync" method
/// </remarks>
public class PolicyService
{

    #region Private properties

    /// <summary>
    /// To check authentication state of the current user, roles, claims ...
    /// </summary>
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    /// <summary>
    /// To check policies for authenticate user
    /// </summary>
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// Keep a reference to the authentication state of user to avoid async call to <see cref="_authenticationStateProvider"/>
    /// </summary>
    private AuthenticationState _currentUserAuthenticationSate;

    #endregion

    #region Initialization

    /// <summary>
    /// PolicyServices intialisation
    /// </summary>
    /// <param name="authenticationStateProvider">AuthenticationState to retrieve authenticated user</param>
    /// <param name="authorizationService">AuthorizationService to check if user match a policy</param>
    public PolicyService(AuthenticationStateProvider authenticationStateProvider, IAuthorizationService authorizationService)
    {
        _authenticationStateProvider = authenticationStateProvider;
        _authorizationService = authorizationService;
    }

    #endregion

    #region methods

    /// <summary>
    /// Check if current user is in a specified policy
    /// </summary>
    /// <param name="user">Informations about the user.</param>
    /// <param name="policy">Policy name to check</param>
    /// <returns><c>true</c> is user is in policy or if the <paramref name="policy"/> is null or empty; <c>false</c> otherwise</returns>
    public async Task<bool> IsInPolicyAsync(ClaimsPrincipal user, string policy)
    {
        if (!string.IsNullOrEmpty(policy))
        {
            if (user is null)
            {
                return false;
            }
            bool isNegate = policy.StartsWith("!");
            if (isNegate)
            {
                policy = policy[1..];
            }
            bool hasPolicy;
            if (policy == "*")
            {
                hasPolicy = true;
            } else
            {
                // Check if this policy has already been check
                hasPolicy = (await _authorizationService.AuthorizeAsync(user, policy)).Succeeded;
            }
            return isNegate ? !hasPolicy : hasPolicy;
        }
        // No policy specified = Authorized
        return true;
    }

    #endregion

    #region Obsolete methods

    /// <summary>
    /// Check if current user is in a specified policy
    /// </summary>
    /// <param name="policy">Policy name to check</param>
    /// <returns><c>true</c> is user is in policy or if the <paramref name="policy"/> is null or empty; <c>false</c> otherwise</returns>
    [Obsolete("Use the \"IsInPolicyAsync\" method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<bool> IsInPolicy(string policy)
    {
        return IsInPolicyAsync(policy);
    }

    /// <summary>
    /// Check if current user is in a specified policy
    /// </summary>
    /// <param name="policy">Policy name to check</param>
    /// <returns><c>true</c> is user is in policy or if the <paramref name="policy"/> is null or empty; <c>false</c> otherwise</returns>
    [Obsolete("Use the \"IsInPolicyAsync\" method by specifying the \"context.user\" of an <LgAuthorizeView><Authorized context>.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<bool> IsInPolicyAsync(string policy)
    {
        return await IsInPolicyAsync(await GetUserAsync(), policy);
    }

    /// <summary>
    /// Return user ClaimsPrincipal
    /// </summary>
    /// <returns>User's claims if authenticated, null otherwise</returns>
    [Obsolete("Use the \"GetUserAsync\" method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    private Task<ClaimsPrincipal> GetUser()
    {
        return GetUserAsync();
    }

    /// <summary>
    /// Return user ClaimsPrincipal
    /// </summary>
    /// <returns>User's claims if authenticated, null otherwise</returns>
    [Obsolete("Use the \"GetUserAsync\" method.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    private async Task<ClaimsPrincipal> GetUserAsync()
    {
        // Retrieve user authentication state
        //if (_currentUserAuthenticationSate == null) RQ: Commented because it can lead to problem when authentication state change 
        //{
        _currentUserAuthenticationSate = await _authenticationStateProvider?.GetAuthenticationStateAsync();
        //}
        // Return user
        if (_currentUserAuthenticationSate != null && _currentUserAuthenticationSate.User != null && _currentUserAuthenticationSate.User.Identity.IsAuthenticated)
        {
            return _currentUserAuthenticationSate.User;
        }
        return null;
    }

    #endregion

}
