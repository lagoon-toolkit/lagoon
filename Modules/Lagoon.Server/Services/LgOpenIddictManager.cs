using Lagoon.Core;
using Lagoon.Model.Context;
using Lagoon.Server.Helpers;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Lagoon.Server.Services;

internal class LgOpenIddictManager<TEFCA, TKey, TAuthorization, TToken> : ILgOpenIddictManager
    where TEFCA : OpenIddictEntityFrameworkCoreApplication<TKey, TAuthorization, TToken>
    where TKey : IEquatable<TKey>
    where TAuthorization : class
    where TToken : class
{

    ///<inheritdoc/>
    public async Task<List<string>> GetRedirectUrisAsync(IServiceProvider serviceProvider, string clientAppId)
    {
        OpenIddictApplicationManager<TEFCA> oim = serviceProvider.GetRequiredService<OpenIddictApplicationManager<TEFCA>>();
        TEFCA oidApp = await oim.FindByClientIdAsync(clientAppId);
        return oidApp is null ? new() : JsonSerializer.Deserialize<List<string>>(oidApp.RedirectUris);
    }

    ///<inheritdoc/>
    public async Task InitializeOpenIddictAsync(IServiceProvider serviceProvider, IApplicationInformation applicationInformation, CancellationToken cancellationToken)
    {
        bool isDevelopment = applicationInformation.IsDevelopment;
        string appName = applicationInformation.RootName;
        string clientAppId = $"{appName}.Client";
        AuthenticationOptions authOptions = serviceProvider.GetRequiredService<AuthenticationOptions>();
        string issuerUri = authOptions.IssuerUri;
        OpenIddictApplicationManager<TEFCA> oidAppManager =
            serviceProvider.GetRequiredService<OpenIddictApplicationManager<TEFCA>>();
        List<string> hosts = new();
        if (isDevelopment)
        {
            hosts.AddRange(LaunchSettings.GetHttpHosts(applicationInformation.PublicURL.StartsWith("https://")));
        }
        else
        {
            // In Production we have to retrieve the PublicUrl from the appSettings (since we are not able to know the baseUrl)
            if (string.IsNullOrEmpty(issuerUri))
            {
                throw new Exception("Missing parameter in appSettings.json. You must configure the 'Lagoon:PublicUrl'.");
            }
            // Add the IssuerUri from appSettings
            hosts.Add(issuerUri);
        }
        TEFCA oidApp = await oidAppManager.FindByClientIdAsync(clientAppId, cancellationToken);
        if (oidApp is not null)
        {
            // there is already a definition for the client application
            // ensure the PublicUrl for this app is registered in the redirectUris
            List<string> foundRedirects = new();
            foreach (string baseUrl in hosts)
            {
                if (oidApp.RedirectUris.Contains($"{baseUrl}/authentication/login-callback"))
                {
                    foundRedirects.Add(baseUrl);
                }
            }
            IEnumerable<string> redirectToAdd = hosts.Where(x => !foundRedirects.Contains(x));
            if (redirectToAdd.Any())
            {
                List<string> redirectUris = JsonSerializer.Deserialize<List<string>>(oidApp.RedirectUris);
                List<string> postLogoutRedirectUris = JsonSerializer.Deserialize<List<string>>(oidApp.PostLogoutRedirectUris);
                foreach (string url in redirectToAdd)
                {
                    redirectUris.Add($"{url}/authentication/login-callback");
                    postLogoutRedirectUris.Add($"{url}/identity/account/signout");
                }
                oidApp.RedirectUris = JsonSerializer.Serialize(redirectUris);
                oidApp.PostLogoutRedirectUris = JsonSerializer.Serialize(postLogoutRedirectUris);
                await oidAppManager.UpdateAsync(oidApp, cancellationToken);
            }
        }
        else
        {
            // Create the default application descriptor 
            OpenIddictApplicationDescriptor app = new()
            {
                ClientId = clientAppId,
                ConsentType = ConsentTypes.Implicit,
                DisplayName = $"{appName} SPA client application",
                Type = ClientTypes.Public,
                Permissions =
                    {
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Logout,
                        Permissions.Endpoints.Token,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,
                        Permissions.ResponseTypes.Code,
                        Permissions.Scopes.Email,
                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Roles
                    },
                Requirements =
                    {
                        Requirements.Features.ProofKeyForCodeExchange
                    }
            };
            // Add the required login/logout callback uri declaration
            foreach (string baseUrl in hosts)
            {
                app.RedirectUris.Add(new Uri($"{baseUrl}/authentication/login-callback"));
                app.PostLogoutRedirectUris.Add(new Uri($"{baseUrl}/identity/account/signout"));
            }
            // Create the app
            await oidAppManager.CreateAsync(app, cancellationToken);

            // Add additionnal configured clients
            foreach (OidcClientDescriptor extClient in authOptions.AdditionnalClients)
            {
                TEFCA oidExt = await oidAppManager.FindByClientIdAsync(extClient.ClientId, cancellationToken);
                if (oidExt is not null)
                {
                    await oidAppManager.DeleteAsync(oidExt, cancellationToken);
                }
                OpenIddictApplicationDescriptor clientDescriptor = new()
                {
                    ClientId = extClient.ClientId,
                    ClientSecret = extClient.ClientSecret,
                    ConsentType = ConsentTypes.Implicit,
                    DisplayName = $"{extClient.ClientId} external client for {appName}",
                    Type = string.IsNullOrEmpty(extClient.ClientSecret) ? ClientTypes.Public : ClientTypes.Confidential,
                    Permissions =
                        {
                            Permissions.Endpoints.Token,
                            Permissions.Scopes.Roles
                        }
                };
                if (extClient.RedirectUris != null)
                {
                    foreach (string redirectUri in extClient.RedirectUris)
                    {
                        if (isDevelopment && !redirectUri.StartsWith("http"))
                        {
                            foreach (string host in hosts)
                            {
                                clientDescriptor.RedirectUris.Add(new Uri($"{host}{redirectUri}"));
                            }
                        }
                        else
                        {
                            clientDescriptor.RedirectUris.Add(new Uri(redirectUri.StartsWith("http") ? redirectUri : $"{issuerUri}{redirectUri}"));
                        }
                    }
                }
                foreach (string permission in extClient.PermissionsGrantType)
                {
                    clientDescriptor.Permissions.Add(permission);
                }
                await oidAppManager.CreateAsync(clientDescriptor, cancellationToken);
            }
        }
    }

}
