using Lagoon.Model.Context;

namespace Lagoon.Server.Application;

/// <summary>
/// Describe Lagoon authentication option
/// </summary>
public class AuthenticationOptions
{

    #region constants

    private const string PasswordRulesSettingsKey = "Lagoon:Authentication:PasswordRules";
    private const string SignInRulesSettingsKey = "Lagoon:Authentication:SignInRules";
    private const string IssuerUriSettingsKey = "Lagoon:Authentication:IssuerUri";
    private const string PublicOriginSettingsKey = "Lagoon:Authentication:PublicOrigin";

    #endregion

    #region fields

    private string _loginUrl = "/Identity/Account/Login";

    #endregion

    #region properties

    /// <summary>
    /// Enable / Disable the "Allow remember me" option when login. Default is <c>true</c>
    /// </summary>
    public bool AllowRememberMe { get; set; } = true;

    /// <summary>
    /// Gets or sets the list of allowed characters in the username used to validate user names. All characters are allowed by defaults.
    /// </summary>
    /// <value>
    /// The list of allowed characters in the username used to validate user names.
    /// </value>
    public string AllowedUserNameCharacters { get; set; }

    /// <summary>
    /// Enable / Disable Windows authentication
    /// (False by default)
    /// </summary>
    public bool AllowWindowsAuthentication { get; set; }

    /// <summary>
    /// Enable / Disable Forms authentication
    /// (False by default)
    /// </summary>
    public bool AllowFormAuthentication { get; set; }

    /// <summary>
    /// Force user to enable MFA before login
    /// False by default. Currently working with Form authentication
    /// </summary>
    public bool MultiFactorAuthenticationRequired { get; set; }

    /// <summary>
    /// Get or set the name of the role for which an user must configure an MFA. Default is <c>null</c> (no role defined)
    /// </summary>
    public string MultiFactorAuthenticationRoleName { get; set; }

    /// <summary>
    /// Login URL page
    /// </summary>
    public string LoginUrl
    {
        get => _loginUrl;
        set
        {
            if (!string.IsNullOrEmpty(value) && value.StartsWith("~"))
            {
                throw new Exception($"The {nameof(LoginUrl)} can't be empty and must not start with a '~' : \"{value}\".");
            }
            _loginUrl = value;
        }
    }

    /// <summary>
    /// Register URL page
    /// </summary>
    /// <value></value>
    public string RegisterUrl { get; set; }

    /// <summary>
    /// Forgot password URL page
    /// </summary>
    /// <value></value>
    public string ForgotPasswordUrl { get; set; } = "~/Identity/Account/ForgotPassword";

    /// <summary>
    /// Gets or sets the unique name of this server instance, e.g. https://myissuer.com.
    /// If not set, the issuer name is inferred from the request    
    /// </summary>
    /// <value>Unique name of this server instance, e.g. https://myissuer.com</value>
    /// <remarks>The value never ends with a slash.</remarks>
    public string IssuerUri { get; }

    /// <summary>
    ///  Gets or sets the origin of this server instance, e.g. https://myorigin.com. If
    ///  not set, the origin name is inferred from the request Note: Do not set a URL
    ///  or include a path.
    /// </summary>
    /// <value>Origin of this server instance, e.g. https://myorigin.com</value>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the IssuerUri")]
    public string PublicOrigin => IssuerUri;

    /// <summary>
    /// Gets or sets a value indicating whether [allow offline access]. Defaults to false.
    /// </summary>
    public bool AllowOfflineAccess { get; set; } = false;

    /// <summary>
    /// Controls how much time the authentication ticket for From authentication stored in the cookie will remain
    ///  valid from the point it is created The expiration information is stored in the
    ///  protected cookie ticket. Because of that an expired cookie will be ignored even
    ///  if it is passed to the server after the browser should have purged it.
    ///  This is separate from the value of Microsoft.AspNetCore.Http.CookieOptions.Expires,
    ///  which specifies how long the browser will keep the cookie.
    /// </summary>
    public int FormAuthenticationCookieLifeTime { get; set; } = 365;

    /// <summary>
    /// Controls how much time the authentication ticket for Windows/SSO authentication stored in the cookie will remain
    ///  valid from the point it is created The expiration information is stored in the
    ///  protected cookie ticket. Because of that an expired cookie will be ignored even
    ///  if it is passed to the server after the browser should have purged it.
    ///  This is separate from the value of Microsoft.AspNetCore.Http.CookieOptions.Expires,
    ///  which specifies how long the browser will keep the cookie.
    /// </summary>
    public int SsoAuthenticationCookieLifeTime { get; set; } = 1;

    /// <summary>
    /// Lifetime of access token in seconds (defaults to 3600 seconds / 1 hour)
    /// </summary>
    public int AccessTokenLifetime { get; set; } = 3600;

    /// <summary>
    /// Additional claims to include in user information profile
    /// (Alls claims added with AddInfo("ClaimName", "ClaimValue") must be declared here)
    /// </summary>
    public IEnumerable<string> CustomClaims { get; set; }

    /// <summary>
    /// Password options (password length, strenght etc)
    /// </summary>
    public PasswordOptions PasswordOptions { get; set; } = new PasswordOptions()
    {
        RequiredLength = 6,
        RequireDigit = true,
        RequiredUniqueChars = 1,
        RequireLowercase = true,
        RequireNonAlphanumeric = true,
        RequireUppercase = true
    };

    /// <summary>
    /// SignIn options (email / phone / ... confirmation)
    /// </summary>
    public SignInOptions SignInOptions { get; set; } = new SignInOptions()
    {
        RequireConfirmedAccount = false,
        RequireConfirmedEmail = false,
        RequireConfirmedPhoneNumber = false
    };

    /// <summary>
    /// Optional additional client
    /// (Use this to allow external(s) application(s) to access to your api)
    /// </summary>
    internal List<OidcClientDescriptor> AdditionnalClients { get; set; } = new List<OidcClientDescriptor>();

    /// <summary>
    /// If the identity of the user must be getted from claims, specify here the type of claim to use.
    /// Example : ClaimTypes.Email.
    /// By default the value is <c>null</c> and the value is getted from the "Identity.Name".
    /// </summary>
    public string SSOUniqueIdentifier { get; set; }

    /// <summary>
    /// Use the default data protection service.
    /// </summary>
    public bool UseDefaultDataProtection { get; set; } = true;

    #endregion

    #region initialization

    /// <summary>
    /// Initialize default authentication options
    /// </summary>
    /// <param name="config">Configuration file</param>
    /// <param name="publicURL">The application public URI.</param>
    public AuthenticationOptions(IConfiguration config, string publicURL)
    {
        // Overide default values with appsettings.json if the sections exists
        config.GetSection(PasswordRulesSettingsKey).Bind(PasswordOptions);
        config.GetSection(SignInRulesSettingsKey).Bind(SignInOptions);
        IssuerUri = NotEmptyValue(config, IssuerUriSettingsKey) ?? NotEmptyValue(config, PublicOriginSettingsKey) ?? publicURL;
        IssuerUri = IssuerUri?.TrimEnd('/');
    }

    /// <summary>
    /// Get a not empty value from the config.
    /// </summary>
    /// <param name="config">Configuration file.</param>
    /// <param name="key">The config key.</param>
    private static string NotEmptyValue(IConfiguration config, string key)
    {
        string value = config.GetValue<string>(key, null);
        return value == "" ? null : value;
    }

    #endregion

}
