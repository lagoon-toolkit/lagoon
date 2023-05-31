using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Security.Claims;

namespace Lagoon.UI.Application.Auth;



/// <summary>
/// / when a user belongs to multiple roles, IS4 returns a single claim with a serialised array of values this class improves the original factory by deserializing the claims in the correct way
/// </summary>
/// <typeparam name="TAccount"></typeparam>
public class ArrayClaimsPrincipalFactory<TAccount> : AccountClaimsPrincipalFactory<TAccount> where TAccount : RemoteUserAccount
{

    /// <summary>
    /// Initialise new instance.
    /// </summary>
    /// <param name="accessor"></param>
    public ArrayClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor) : base(accessor)
    {
    }

    /// <summary>
    /// Create ClaimsPrincipal from account
    /// </summary>
    /// <param name="account">User informations</param>
    /// <param name="options">Authentication options</param>
    /// <returns>ClaimsPrincipal</returns>
    public async override ValueTask<ClaimsPrincipal> CreateUserAsync(TAccount account, RemoteAuthenticationUserOptions options)
    {
        // Create base claims
        ClaimsPrincipal user = await base.CreateUserAsync(account, options);
        // Retrieve identity
        var claimsIdentity = (ClaimsIdentity)user.Identity;

        if (account != null)
        {
            foreach (var kvp in account.AdditionalProperties)
            {
                var name = kvp.Key;
                var value = kvp.Value;

                // Transform role: ['', '', ...] into role: '', role:'', ...
                if (value != null && (value is JsonElement element && element.ValueKind == JsonValueKind.Array))
                {
                    claimsIdentity.RemoveClaim(claimsIdentity.FindFirst(kvp.Key));

                    var claims = element.EnumerateArray().Select(x => new Claim(kvp.Key, x.ToString()));
                    claimsIdentity.AddClaims(claims);
                }
            }
        }

        return user;
    }
}
