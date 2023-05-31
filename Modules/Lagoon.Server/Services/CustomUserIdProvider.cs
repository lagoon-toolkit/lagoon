using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Lagoon.Server.Services;

/// <summary>
/// Service to get the context User ID.
/// </summary>
public class CustomUserIdProvider : IUserIdProvider
{
    /// <summary>
    /// Gets the user identifiant.
    /// </summary>
    /// <param name="connection">Hub connection context.</param>
    /// <returns>The user identifiant.</returns>
    public string GetUserId(HubConnectionContext connection)
    {
        // Get userId
        return connection.User.FindFirstValue(Claims.Subject);
    }
}
