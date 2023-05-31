using Lagoon.Model.Context;
using Lagoon.Server.Application.IdentitySources;
using System.Security.Claims;

namespace Lagoon.Server.Application;

public abstract partial class LgAuthApplication<TDbContext, TUser>
{

    #region "AuthenticationContext" protected class

    /// <summary>
    /// The authentication context.
    /// </summary>
    public class AuthenticationContext
    {

        #region fields

        /// <summary>
        /// List of role to apply to the sigin process
        /// </summary>
        private List<string> _customsRoles;

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets if the "OnUnknownUserAsync" method of the "main.cs" will be called if the user login is unknown for the application.
        /// By default the value is <c>false</c> for identitysource with a "AuthenticationMode" to "Forms"; <c>true</c> for other authentication mode.
        /// </summary>
        /// <remarks>This value can be changed in the "GetIdentityUserAsync" of the "main.cs" method.</remarks>
        public bool AllowUnknownUser { get; set; }

        /// <summary>
        /// List of additional claims that must be inclued in tokens.
        /// </summary>
        public List<Claim> CustomsClaims { get; set; } = new();

        /// <summary>
        /// List of role to apply to the sigin process
        /// </summary>
        public List<string> CustomsRoles
        {
            get
            {
                if (!IsCustomRolesEnabled)
                {
                    throw new Exception("[IdentitySource] 'EnabledCustomRoles' is false. 'CustomsRoles' cannot be set");
                }

                _customsRoles ??= new();
                return _customsRoles;
            }
        }

        /// <summary>
        /// The database context.
        /// </summary>
        public TDbContext DbContext { get; }


        /// <summary>
        /// Source information that identifies the user.
        /// </summary>
        public IdentitySource IdentitySource { get; }

        /// <summary>
        /// Gets or sets if the custom roles must replace the existing users roles during the signin process.
        /// </summary>
        public bool IsCustomRolesEnabled { get; set; }

        /// <summary>
        /// Gets or sets the
        /// </summary>
        internal Type GroupChoiceEnumType { get; set; }

        /// <summary>
        /// Gets or sets if the "Email" property of an user must be used to identify the user.
        /// By default the value is <c>true</c> for identitysource with a "AuthenticationMode" to "SSO"; <c>false</c> for other authentication mode.
        /// </summary>
        public bool IsEmailLogin { get; set; }

        /// <summary>
        /// The authentication scope service provider.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Gets the user login from the identity source.
        /// </summary>
        public string UserLogin => IdentitySource.UserLogin;

        /// <summary>
        /// The user manager.
        /// </summary>
        public UserManager<TUser> UserManager { get; }

        #endregion

        #region constructors

        /// <summary>
        /// New instance.
        /// </summary>
        /// <param name="serviceProvider">The authentication scope service provider.</param>
        /// <param name="identitySource">Source information that identifies the user.</param>
        public AuthenticationContext(IServiceProvider serviceProvider, IdentitySource identitySource)
        {
            ServiceProvider = serviceProvider;
            IdentitySource = identitySource;
            DbContext = (TDbContext)serviceProvider.GetRequiredService<ILgApplicationDbContext>();
            UserManager = serviceProvider.GetRequiredService<UserManager<TUser>>();
            // By default Forms authentication don't allow unknown user
            AllowUnknownUser = identitySource.AuthenticationMode == AuthenticationMode.Forms;
        }

        #endregion

        #region methods

        /// <summary>
        /// Add a role to an user.
        /// </summary>
        /// <param name="roles">List of role names.</param>
        /// <remarks>IsCustomRole MUST BE ENABLED BEFORE calling this  method.</remarks>
        /// <returns>this for chainning</returns>
        public AuthenticationContext AddCustomRole(params string[] roles)
        {
            return AddCustomRoles(roles.AsEnumerable());
        }

        /// <summary>
        /// Add a role to an user.
        /// </summary>
        /// <param name="roles">List of role names.</param>
        /// <remarks>IsCustomRole MUST BE ENABLED BEFORE calling this  method.</remarks>
        /// <returns>this for chainning</returns>
        public AuthenticationContext AddCustomRoles(IEnumerable<string> roles)
        {
            CustomsRoles.AddRange(roles);
            return this;
        }

        /// <summary>
        /// Add custom claims to user
        /// </summary>
        /// <param name="key">Claim key</param>
        /// <param name="value">Claim value</param>
        /// <returns>this for chainning</returns>
        public AuthenticationContext AddCustomClaim(string key, string value)
        {
            CustomsClaims.Add(new Claim(key, value));
            return this;
        }

        /// <summary>
        /// Add a selectable profile.
        /// </summary>
        /// <param name="description">Profile description.</param>
        /// <param name="roles">List of roles applicable for this profile.</param>
        public void AddRoleChoice(string description, params string[] roles)
        {
            IdentitySource.AddRoleChoice(description, roles);
        }

        /// <summary>
        /// Add a screen to the login process to allow user to choose only one of his role.
        /// </summary>
        /// <typeparam name="TRoles"></typeparam>
        public void EnableGroupChoice<TRoles>()
        {
            GroupChoiceEnumType = typeof(TRoles);
        }

        #endregion

    }

    #endregion

    #region "InternalAuthenticationContext" private class

    /// <summary>
    /// The authentication context.
    /// </summary>
    private class InternalAuthenticationContext : AuthenticationContext
    {

        #region properties

        /// <summary>
        /// The authentication options.
        /// </summary>
        public AuthenticationOptions Options { get; }

        /// <summary>
        /// The authentication manager.
        /// </summary>
        public AuthenticationManager<TUser> Manager { get; }

        #endregion

        #region constructors

        /// <summary>
        /// New instance.
        /// </summary>
        /// <param name="serviceProvider">The authentication scope service provider.</param>
        /// <param name="identitySource">Source information that identifies the user.</param>
        public InternalAuthenticationContext(IServiceProvider serviceProvider, IdentitySource identitySource)
            : base(serviceProvider, identitySource)
        {
            AllowUnknownUser = identitySource.AuthenticationMode != AuthenticationMode.Forms;
            IsEmailLogin = identitySource.AuthenticationMode == AuthenticationMode.SSO;
            Manager = serviceProvider.GetRequiredService<AuthenticationManager<TUser>>();
            Options = serviceProvider.GetRequiredService<AuthenticationOptions>();
        }

        #endregion

    }

    #endregion

}
