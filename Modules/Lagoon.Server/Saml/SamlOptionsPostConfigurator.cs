using Microsoft.Extensions.Options;
using AuthenticationOptions = Microsoft.AspNetCore.Authentication.AuthenticationOptions;

namespace Lagoon.Server.Saml;

/// <summary>
/// Post configure service to set default values in configuration if the startup didn't set them.
/// </summary>
internal class SamlOptionsPostConfigurator : IPostConfigureOptions<SamlOptions>
{

    #region fields

    /// <summary>
    /// The main application.
    /// </summary>
    private readonly ILgApplication _app;

    /// <summary>
    /// Authentication options to find the default sign in scheme.
    /// </summary>
    private IOptions<AuthenticationOptions> _authOptions;

    #endregion

    #region constructors

    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <param name="authOptions"></param>
    public SamlOptionsPostConfigurator(ILgApplication app, IOptions<AuthenticationOptions> authOptions)
    {
        _app = app;
        _authOptions = authOptions;
    }

    #endregion

    #region methods

    /// <summary>
    /// Add defaults to configuration.
    /// </summary>
    /// <param name="name">The name of the options.</param>
    /// <param name="options">The options.</param>
    public void PostConfigure(string name, SamlOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        // Set default value
        options.EntityId ??= _app.ApplicationInformation.PublicURL;
        options.SignInScheme ??= _authOptions.Value.DefaultSignInScheme ?? _authOptions.Value.DefaultScheme;
        options.SignOutScheme ??= _authOptions.Value.DefaultSignOutScheme ?? _authOptions.Value.DefaultAuthenticateScheme;
    }

    #endregion

}
