using Lagoon.Model.Context;
using Lagoon.Server.Application.Authentication;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Lagoon.Server.Application;


/// <summary>
/// Helper extension methods
/// </summary>
public static class LagoonExtension
{

    /// <summary>
    /// Helper for adding an external M2M client type access to the application.
    /// Use this method to allow an external application to authenticate to the application without user account 
    /// </summary>
    /// <param name="options">AuthenticationOptions extension</param>
    /// <param name="clientId">ClientID</param>
    /// <param name="clientSecret">ClientSecret</param>
    /// <param name="scope">Project scope</param>
    /// <param name="roles">Role associated to the client</param>
    /// <returns>this for chaining</returns>
    public static AuthenticationOptions AddClientWithCredential(this AuthenticationOptions options, string clientId, string clientSecret, string scope, params string[] roles)
    {
        if (string.IsNullOrEmpty(clientSecret))
        {
            // In M2M scenario ensure that the clientId have a secret value
            throw new InvalidOperationException($"A 'clientSecret' must be set for '{clientId}'");
        }
        // Add an external identity with clientId/clientSecret and specific roles
        options.AdditionnalClients.Add(new OidcClientDescriptor(new string[] { Permissions.GrantTypes.ClientCredentials }, clientId, clientSecret, scope, roles, null));
        return options;
    }

    /// <summary>
    /// Helper for adding client to application requiring code grant
    /// Use this method to allow an external application to authenticate to the application with a user account
    /// </summary>
    /// <param name="options">AuthenticationOptions extension</param>
    /// <param name="clientId">ClientID</param>
    /// <param name="clientSecret">ClientSecret</param>
    /// <param name="scope">Project scope</param>
    /// <param name="redirectUri">URIs to return tokens or authorization codes to</param>
    /// <returns>this for chaining</returns>
    public static AuthenticationOptions AddClientWithCredentialAndCode(this AuthenticationOptions options, string clientId, string clientSecret, string scope, params string[] redirectUri)
    {
        // Add an external identity for a client with cliendId/clientSecret + authorization code through the Identity/Account/Login page
        options.AdditionnalClients.Add(new OidcClientDescriptor(
            new string[] {
                Permissions.GrantTypes.ClientCredentials,
                Permissions.Endpoints.Authorization,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.ResponseTypes.Code
            },
            clientId, clientSecret, scope, null, redirectUri));
        return options;
    }

    /// <summary>
    /// Open an access for Swagger
    /// </summary>
    /// <param name="options">AuthenticationOptions extension method</param>
    /// <param name="clientId">This clientId will be used to fire the authorization flow in swagger ui (no password required since it use the connected user credential)</param>
    public static AuthenticationOptions AddSwaggerAccess(this AuthenticationOptions options, string clientId = "SwaggerUI")
    {
        // Configure an external identity for swagger (cliend_id:SwaggerUI, secret:null
        AddClientWithCredentialAndCode(options, clientId, null, null, "/swagger/oauth2-redirect.html");
        return options;
    }

    /// <summary>
    ///  Configure OpenIddict to use the default entities with a custom key type if TKey is not string.
    /// </summary>
    /// <typeparam name="TKey">The type of keys to use.</typeparam>
    /// <param name="builder">The builder.</param>
    /// <returns>The builder.</returns>
    public static OpenIddictEntityFrameworkCoreBuilder ApplyKeyType<TKey>(this OpenIddictEntityFrameworkCoreBuilder builder)
        where TKey : IEquatable<TKey>
    {
        // Don't replace the default entities if the type is string
        if (typeof(TKey) != typeof(string))
        {
            builder.ReplaceDefaultEntities<TKey>();
        }
        return builder;
    }

}