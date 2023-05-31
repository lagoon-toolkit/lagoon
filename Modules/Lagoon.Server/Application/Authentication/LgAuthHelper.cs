using Lagoon.Model.Models;
using System.Security.Claims;

namespace Lagoon.Server.Application.Authentication;


/// <summary>
/// <see cref="ILgAuthHelper"/> implementation
/// </summary>
/// <typeparam name="TUser">Application user used by the <see cref="UserManager{TUser}"/> and <see cref="SignInManager{TUser}"/></typeparam>
public class LgAuthHelper<TUser> : ILgAuthHelper
    where TUser : class, ILgIdentityUser
{

    #region private vars

    private readonly SignInManager<TUser> _signInManager;
    private readonly UserManager<TUser> _userManager;

    #endregion

    #region initialization

    /// <summary>
    /// Initialise a new <see cref="LgAuthHelper{TUser}"/>
    /// </summary>
    /// <param name="signInManager"></param>
    /// <param name="userManager"></param>
    public LgAuthHelper(SignInManager<TUser> signInManager, UserManager<TUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    #endregion

    /// <inheritdoc />
    public async Task<string> GetUserIdAsync(ClaimsPrincipal principal)
    {
        try
        {
            if (principal.Identity.IsAuthenticated)
            {
                TUser user = await _userManager.GetUserAsync(principal);
                return user == null ? null : await _userManager.GetUserIdAsync(user);
            }
        }
        catch (Exception)
        { }
        return null;
    }

    /// <inheritdoc />
    public async Task<bool> ValidateSecurityStampAsync(ClaimsPrincipal principal)
    {
        return (await _signInManager.ValidateSecurityStampAsync(principal)) != null;
    }

    /// <inheritdoc />
    public async Task<bool> CanSignInAsync(ClaimsPrincipal principal)
    {
        try
        {
            if (principal.Identity.IsAuthenticated)
            {
                TUser user = await _userManager.GetUserAsync(principal);
                return user != null && await _signInManager.CanSignInAsync(user);
            }
        }
        catch (Exception)
        { }
        return false;
    }

    /// <inheritdoc />
    public async Task<ClaimsPrincipal> CreateUserPrincipalAsync(string userId)
    {
        TUser user = await _userManager.FindByIdAsync(userId);
        return user != null ? await _signInManager.CreateUserPrincipalAsync(user)
            : throw new InvalidOperationException("Unable to create userPrincipal for the specifier userId");
    }

    /// <inheritdoc />
    public Task SignOutAsync()
    {
        return _signInManager.SignOutAsync();
    }
}
