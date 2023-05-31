using Lagoon.Core;

namespace Lagoon.Server.Services;

/// <summary>
/// The Lagoon's OpenIddict manager.
/// </summary>
public interface ILgOpenIddictManager
{

    /// <summary>
    /// Get the already registred uris.
    /// </summary>
    /// <param name="serviceProvider">A scoped service provider.</param>
    /// <param name="clientAppId"></param>
    /// <returns>The already registred uris.</returns>
    Task<List<string>> GetRedirectUrisAsync(IServiceProvider serviceProvider, string clientAppId);


    /// <summary>
    /// Initialize the OpenIddict application manager for Blazor hosted application (eg. the authorization server co-exist with the APIs)
    /// </summary>
    /// <param name="serviceProvider">A scoped service provider.</param>
    /// <param name="applicationInformation">The application's informations.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task InitializeOpenIddictAsync(IServiceProvider serviceProvider, IApplicationInformation applicationInformation, CancellationToken cancellationToken);

}