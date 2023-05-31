using System.Security.Claims;

namespace Lagoon.Server.Application.IdentitySources;

/// <summary>
/// Create a new identity source by impersonnated the specified login.
/// </summary>
public class ImpersonatedIdentity : IdentitySource
{


    #region constants

    /// <summary>
    /// The identifier of this identity source.
    /// </summary>
    public const string SOURCE_NAME = "Impersonated";

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="login">The user login.</param>
    /// <param name="claims">The list of claims to provide to the authentication.</param>
    public ImpersonatedIdentity(string login, params Claim[] claims)
        : this(login, claims.AsEnumerable())
    { }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="login">The user login.</param>
    /// <param name="firstName">Define the <c>ClaimTypes.GivenName</c> claim type value.</param>
    /// <param name="lastName">Define the <c>ClaimTypes.Surname</c> claim type value.</param>
    /// <param name="email">Define the <c>ClaimTypes.Email</c> claim type value.</param>
    /// <param name="claims">The list of claims to provide to the authentication.</param>
    public ImpersonatedIdentity(string login, string firstName, string lastName, string email = null, params Claim[] claims)
        : this(login, JoinNamesToClaims(firstName, lastName, email, claims))
    { }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="login">The user login.</param>
    /// <param name="claims">The list of claims to provide to the authentication.</param>
    public ImpersonatedIdentity(string login, IEnumerable<Claim> claims)
        : this(AuthenticationMode.SSO, login, claims)
    { }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="authenticationMode">The simulated authentication mode.</param>
    /// <param name="login">The user login.</param>
    /// <param name="claims">The list of claims to provide to the authentication.</param>
    public ImpersonatedIdentity(AuthenticationMode authenticationMode, string login, IEnumerable<Claim> claims)
        : this(authenticationMode, login, claims, SOURCE_NAME)
    { }

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="authenticationMode">The simulated authentication mode.</param>
    /// <param name="login">The user login.</param>
    /// <param name="claims">The list of claims to provide to the authentication.</param>
    /// <param name="sourceName">The name.</param>
    private ImpersonatedIdentity(AuthenticationMode authenticationMode, string login, IEnumerable<Claim> claims, string sourceName)
        : base(authenticationMode, login, claims, sourceName, false)
    { }


    #endregion

    #region static methods

    /// <summary>
    /// New instance.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    public static ImpersonatedIdentity FromConfiguration(IConfiguration configuration)
    {
        var config = configuration.GetSection("Impersonate");
        // Get the login
        string login = config["Login"];
        // Check if the impersonation is disabled
        if (string.IsNullOrEmpty(login) || string.Equals(config["Enabled"], "false", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
        // Get the authentication mode
        if (!Enum.TryParse(config["AuthenticationMode"], true, out AuthenticationMode authenticationMode))
        {
            authenticationMode = AuthenticationMode.SSO;
        }
        // Get the source name
        string sourceName = config["SourceName"] ?? SOURCE_NAME;
        // Load claims
        List<Claim> claims = new();
        // Get the known claims
        AddKnownClaim(claims, config, "FirstName", ClaimTypes.GivenName);
        AddKnownClaim(claims, config, "LastName", ClaimTypes.Surname);
        AddKnownClaim(claims, config, "Email", ClaimTypes.Email);
        AddKnownClaim(claims, config, "DisplayName", "http://schemas.microsoft.com/identity/claims/displayname");
        // Get the custom claims

        // Get the fully defined claims
        return new(authenticationMode, login, claims, sourceName);
    }

    private static void AddKnownClaim(List<Claim> claims, IConfigurationSection config, string settingName, string claimType)
    {
        string value = config[settingName];
        if (!string.IsNullOrEmpty(value))
        {
            claims.Add(new Claim(claimType, value));
        }
    }

    #endregion

    #region methods

    /// <summary>
    /// Return the complete list of claims.
    /// </summary>
    /// <param name="firstName">Define the <c>ClaimTypes.GivenName</c> claim type value.</param>
    /// <param name="lastName">Define the <c>ClaimTypes.Surname</c> claim type value.</param>
    /// <param name="email">Define the <c>ClaimTypes.Email</c> claim type value.</param>
    /// <param name="claims">The list of claims to provide to the authentication.</param>
    /// <returns>The complete list of claims.</returns>
    private static IEnumerable<Claim> JoinNamesToClaims(string firstName, string lastName, string email = null, params Claim[] claims)
    {
        if (!string.IsNullOrEmpty(firstName))
        {
            yield return new Claim(ClaimTypes.GivenName, firstName);
        }
        if (!string.IsNullOrEmpty(lastName))
        {
            yield return new Claim(ClaimTypes.Surname, lastName);
        }
        if (!string.IsNullOrEmpty(email))
        {
            yield return new Claim(ClaimTypes.Email, email);
        }
        foreach (Claim claim in claims)
        {
            yield return claim;
        }
    }

    #endregion

}
