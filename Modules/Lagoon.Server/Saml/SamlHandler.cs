using Lagoon.Server.Saml.Messages;
using Lagoon.Server.Saml.Messages.Custom;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Security.Claims;

namespace Lagoon.Server.Saml;


/// <summary>
/// Authentication handler for SAML2.
/// </summary>
internal class SamlHandler : IAuthenticationRequestHandler, IAuthenticationSignOutHandler
{

    #region constants

    /// <summary>
    /// The route of the SAML2 handler.
    /// </summary>
    private const string ROUTE = "/saml";

    /// <summary>
    /// The relay state is valid 15 minutes.
    /// </summary>
    private const int RELAY_STATE_LIFETIME = 15;

    #endregion

    #region fields

    private readonly ILgApplication _app;
    private readonly SamlMetadataProvider _metadataProvider;
    private readonly IOptionsMonitorCache<SamlOptions> _optionsCache;
    private readonly ITimeLimitedDataProtector _dataProtector;
    private readonly IOptionsFactory<SamlOptions> _optionsFactory;
    private AuthenticationScheme _scheme;
    private HttpContext _httpContext;

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="app">The main application.</param>
    /// <param name="metadataProvider">The identity provider metadata provider.</param>
    /// <param name="dataProtectorProvider">Data Protector Provider</param>
    /// <param name="optionsCache">Options</param>
    /// <param name="optionsFactory">Factory for options</param>
    public SamlHandler(ILgApplication app, SamlMetadataProvider metadataProvider, IDataProtectionProvider dataProtectorProvider,
        IOptionsMonitorCache<SamlOptions> optionsCache, IOptionsFactory<SamlOptions> optionsFactory)
    {
        ArgumentNullException.ThrowIfNull(dataProtectorProvider);
        _app = app;
        _metadataProvider = metadataProvider;
        _dataProtector = dataProtectorProvider.CreateProtector(GetType().FullName).ToTimeLimitedDataProtector();
        _optionsFactory = optionsFactory;
        _optionsCache = optionsCache;
    }

    #endregion

    #region methods

    /// <summary>
    /// Initialize for a new HTTP context.
    /// </summary>
    /// <param name="scheme">Authentication handler and his name.</param>
    /// <param name="httpContext">The HTTP context.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public Task InitializeAsync(AuthenticationScheme scheme, HttpContext httpContext)
    {
        _scheme = scheme;
        _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        return Task.CompletedTask;
    }

    /// <InheritDoc />
    [ExcludeFromCodeCoverage]
    public Task<AuthenticateResult> AuthenticateAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Authentication request.
    /// </summary>
    /// <param name="properties">The properties.</param>
    public async Task ChallengeAsync(AuthenticationProperties properties)
    {
        // Get the options from configuration
        SamlOptions options = GetOptions();
        // Initialize the SP metadata
        SpMetadata spMetadata = GetSpMetadata(options);
        // Load the IdP metadata
        IdpMetadata idpMetadata = await GetIdpMetadataAsync(options);
        // Redirect to the IdP with auth request in the query
        SamlAuthnRequest requestBuilder = new(idpMetadata, spMetadata, GetRelayState(properties));
        _httpContext.Response.Redirect(requestBuilder.BuildRequestUrl());
    }

    /// <summary>
    /// Serialize the relay state.
    /// </summary>
    /// <param name="properties">The authentication properties.</param>
    /// <returns>The serialized relay state.</returns>
    private byte[] GetRelayState(AuthenticationProperties properties)
    {
        byte[] data = PropertiesSerializer.Default.Serialize(properties);
        data = SamlSimpleMessage.Compress(data);
        data = _dataProtector.Protect(data, TimeSpan.FromMinutes(RELAY_STATE_LIFETIME));
        return data;
    }

    /// <summary>
    /// Deserialize the relay state.
    /// </summary>
    /// <param name="relayState">The serialized relay state.</param>
    /// <returns>The authentication properties.</returns>
    private AuthenticationProperties GetAuthenticationProperties(byte[] relayState)
    {
        try
        {
            byte[] data = _dataProtector.Unprotect(relayState);
            data = SamlSimpleMessage.Decompress(data);
            return PropertiesSerializer.Default.Deserialize(data);
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to load SAML2 relay state.", ex);
        }
    }

    /// <summary>
    /// Handle the SAML response.
    /// </summary>
    public async Task<bool> HandleRequestAsync()
    {
        if (_httpContext.Request.Path.Equals(ROUTE, StringComparison.Ordinal))
        {
            // Get the options from configuration
            SamlOptions options = GetOptions();
            // Initialize the SP metadata
            SpMetadata spMetadata = GetSpMetadata(options);
            if (_httpContext.Request.Method == "POST")
            {
                // Load the IdP metadata
                IdpMetadata idpMetadata = await GetIdpMetadataAsync(options);
                // Load the response
                SamlResponse response = SamlResponse.FromHttpRequest(idpMetadata, spMetadata.EntityId, _httpContext.Request);
                AuthenticationProperties properties = GetAuthenticationProperties(response.RelayState);
                // Redirect to "Areas\Identity\Pages\Account\ExternalProvidersLogin.cshtml.cs" (SeeOther[303] means POST to GET method)
                _httpContext.Response.StatusCode = (int)HttpStatusCode.SeeOther;
                _httpContext.Response.Headers.Add("Location", properties.RedirectUri);
                // Create identity
                ClaimsIdentity identity = new(response.Attributes.ToClaims(idpMetadata, response.NameId), _scheme.Name, ClaimTypes.NameIdentifier, ClaimTypes.Role);
                // Signin with the SAML response informations
                await _httpContext.SignInAsync(options.SignInScheme, new ClaimsPrincipal(identity), properties);
            }
            else
            {
                //Show the service provider metadata informations for the application
                SpMetatadataMessage message = new(spMetadata);
                message.WriteTo(_httpContext.Response);
            }
            return true;
        }
        return false;
    }

    /// <InheritDoc />
    [ExcludeFromCodeCoverage]
    public Task ForbidAsync(AuthenticationProperties properties)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Initiate a federated sign out if supported (Idp supports it and sp has a configured
    /// signing certificate)
    /// </summary>
    /// <param name="properties">Authentication props, containing a return url.</param>
    [ExcludeFromCodeCoverage]
    public Task SignOutAsync(AuthenticationProperties properties)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get the options.
    /// </summary>
    private SamlOptions GetOptions()
    {
        return _optionsCache.GetOrAdd(_scheme.Name, () => _optionsFactory.Create(_scheme.Name));
    }

    /// <summary>
    /// Get the identity provider metadata.
    /// </summary>
    /// <param name="options">The configuration options.</param>
    /// <returns>The IdP metadata.</returns>
    private Task<IdpMetadata> GetIdpMetadataAsync(SamlOptions options)
    {
        return _metadataProvider.GetIdpMetadataAsync(options.IdpMetadataLocation);
    }

    /// <summary>
    /// Get the service provider (the current application) metadata.
    /// </summary>
    /// <param name="options">The configuration options.</param>
    /// <returns>The SP metadata.</returns>
    private SpMetadata GetSpMetadata(SamlOptions options)
    {
        string publicUrl = _app.ApplicationInformation.PublicURL;
        return new()
        {
            AssertionConsumerLocationUrl = publicUrl + ROUTE[1..],
            EntityId = options.EntityId,
            NameIdFormat = options.NameIdPolicy
        };
    }

    #endregion

}
