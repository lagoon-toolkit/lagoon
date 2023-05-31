using Lagoon.Model.Models;
using System.Security.Claims;

namespace Lagoon.Server.Application.IdentitySources;


/// <summary>
/// Base class to describe an user which attempt to connect
/// </summary>
public class IdentitySource
{

    #region Public properties

    /// <summary>
    /// Gets the kind.
    /// </summary>
    /// <value>
    /// The kind.
    /// </value>
    public AuthenticationMode AuthenticationMode { get; }

    /// <summary>
    /// Gets the claims.
    /// </summary>
    /// <value>
    /// The claims.
    /// </value>
    public IEnumerable<Claim> Claims { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string SourceName { get; }

    /// <summary>
    /// Gets the user login.
    /// </summary>
    /// <value>
    /// The user login.
    /// </value>
    public string UserLogin { get; private set; }

    #endregion

    #region Private properties

    /// <summary>
    /// Custom list of roles (cf. AddRoleChoice(...))
    /// </summary>
    internal List<GroupChoice> GroupChoices = new();

    /// <summary>
    /// If true the authentication ticket will be stored in Cookie,
    /// if false authentication tocket will be store in Session
    /// </summary>
    internal bool RememberMe { get; }

    /// <summary>
    /// An access to the external SSO data response
    /// </summary>
    internal ExternalLoginInfo ExternalLoginInfo { get; set; }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentitySource" /> class.
    /// </summary>
    /// <param name="authenticationMode">The authentication mode.</param>
    /// <param name="login">The login.</param>
    /// <param name="claims">The claims.</param>
    /// <param name="sourceName">The name.</param>
    /// <param name="rememberMe">IdentitySource remember me</param>
    public IdentitySource(AuthenticationMode authenticationMode, string login, IEnumerable<Claim> claims = null, string sourceName = null, bool rememberMe = false)
    {
        AuthenticationMode = authenticationMode;
        UserLogin = login;
        Claims = claims;
        SourceName = sourceName;
        RememberMe = rememberMe;
    }

    #endregion

    #region public methods

    /// <summary>
    /// For debug purpose
    /// </summary>
    /// <param name="login">User login to impersonate</param>
    /// <remarks>OBSOLETE: Use the "GetImpersonatedIdentity" method.</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void Impersonate(string login)
    {
        UserLogin = login;
    }

    /// <summary>
    /// Add a selectable profile.
    /// </summary>
    /// <param name="description">Profile description.</param>
    /// <param name="roles">List of roles applicable for this profile.</param>
    public IdentitySource AddRoleChoice(string description, params string[] roles)
    {
        if (roles == null || roles.Length == 0)
            throw new Exception("GroupChoiceEmpty".Translate(description));
        GroupChoices.Add(new GroupChoice(description, new List<string>(roles)));
        return this;
    }

    /// <summary>
    /// Get list group choices
    /// </summary>
    /// <returns></returns>
    [Obsolete("Use the \"GroupChoices\" property.", true)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<GroupChoice> GetGroupChoices()
    {
        return GroupChoices;
    }

    ///<inheritdoc/>
    public override string ToString()
    {
        return $"{SourceName} (AuthenticationMode.{AuthenticationMode})";
    }

    #endregion

    #region internal methods

    /// <summary>
    /// No validation required.
    /// </summary>
    /// <typeparam name="TUser">User application type</typeparam>
    /// <param name="user">User to check</param>
    /// <param name="userManager">Application user management</param>
    internal protected virtual Task CheckUserAccessAsync<TUser>(TUser user, UserManager<TUser> userManager)
        where TUser : class, ILgIdentityUser
    {
        return Task.CompletedTask;
    }

    #endregion

}
