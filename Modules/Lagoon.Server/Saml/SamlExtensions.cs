using Lagoon.Server.Saml;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using AuthenticationOptions = Microsoft.AspNetCore.Authentication.AuthenticationOptions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for SAML2 authentication.
/// </summary>
public static class SamlExtensions
{

    /// <summary>
    /// Register SAML2 Authentication with default scheme name.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="configureOptions">Action that configures the SAML2 options.</param>
    /// <returns>The authentication builder.</returns>
    public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder, Action<SamlOptions> configureOptions = null)
    {
        return builder.AddSaml("SAML", configureOptions);
    }

    /// <summary>
    /// Register SAML2 Authentication with default scheme name.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The name of the authentication scheme.</param>
    /// <param name="configureOptions">Action that configures the SAML2 options.</param>
    /// <returns>The authentication builder.</returns>
    public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder, string scheme, Action<SamlOptions> configureOptions = null)
    {
        return builder.AddSaml(scheme, "SAML", configureOptions);
    }

    /// <summary>
    /// Register SAML2 Authentication with default scheme name.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <param name="scheme">The name of the authentication scheme.</param>
    /// <param name="displayName">The display name of scheme.</param>
    /// <param name="configureOptions">Action that configures the SAML2 options.</param>
    /// <returns>The authentication builder.</returns>
    public static AuthenticationBuilder AddSaml(this AuthenticationBuilder builder, string scheme, string displayName, Action<SamlOptions> configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<SamlOptions>, SamlOptionsPostConfigurator>());
        builder.Services.Configure<AuthenticationOptions>(o =>
        {
            o.AddScheme(scheme, s =>
            {
                s.HandlerType = typeof(SamlHandler);
                s.DisplayName = displayName;
            });
        });
        if (configureOptions is not null)
        {
            builder.Services.Configure(scheme, configureOptions);
        }
        builder.Services.AddTransient<SamlHandler>();
        builder.Services.AddHttpClient(SamlMetadataProvider.HTTP_CLIENT_NAME);
        builder.Services.AddSingleton<SamlMetadataProvider>();
        return builder;
    }

}
