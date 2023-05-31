using Lagoon.Core.Application;
using Lagoon.Model.Context;
using Lagoon.Model.Models;
using Lagoon.Server.Application.Authentication;
using Lagoon.Server.Application.IdentitySources;
using Lagoon.Server.Helpers;
using Lagoon.Server.Services;
using Lagoon.Server.Services.LagoonSettings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Validation;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreHandlers;

namespace Lagoon.Server.Application;


/// <summary>
/// Lagoon core application with authentication.
/// </summary>
/// <typeparam name="TUser">Application user type.</typeparam>
/// <typeparam name="TDbContext">Default database context for the application.</typeparam>
public abstract partial class LgAuthApplication<TDbContext, TUser> : LgApplication<TDbContext>, ILgAuthApplication
    where TDbContext : DbContext, ILgApplicationDbContext
    where TUser : class, ILgIdentityUser, new()
{

    #region properties

    /// <summary>
    /// List all browser items that need to be removed when a user log out.
    /// </summary>
    public SignOutCleaner SignOutCleaner { get; } = new();

    #endregion

    #region constructors

    /// <summary>
    /// New instance.
    /// </summary>
    public LgAuthApplication()
    { }

    /// <summary>
    /// New instance with a builder.
    /// </summary>
    /// <param name="builder">An additionnal builder class.</param>
    protected LgAuthApplication(ILgApplicationBuilder builder)
        : base(builder)
    { }

    #endregion

    #region Register services

    ///<inheritdoc/>
    protected override void InternalConfigureServices(IServiceCollection services)
    {
        // Register ancestor services
        base.InternalConfigureServices(services);
        // Add access to LgEulaManager
        services.AddScoped<ILgEulaManager, LgEulaManager>();
        var userKeyAdapter = GetUserKeyAdpter();
        // Add access to the Lagoon internal settings manager;
        services.AddScoped(typeof(ILagoonSettingsManager), userKeyAdapter.GetSettingsManagerType());
        // Add access to the Lagoon OpenIddict manager;
        services.AddScoped(typeof(ILgOpenIddictManager), userKeyAdapter.GetLgOpenIddictManagerType());
    }

    ///<inheritdoc/>
    internal override void RegisterAuthentication(IServiceCollection services)
    {
        AuthenticationOptions options = new(Configuration, ApplicationInformation.PublicURL);
        ConfigureAuthentification(options);
        RegisterAuthentication(services, Configuration, options);
    }

    /// <summary>
    /// Configure the authentication.
    /// </summary>
    /// <param name="options">The authentication options.</param>
    /// <remarks>This method should only be overridden by intermediate customization projects (Lagoon.Server.*)</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void ConfigureAuthentification(AuthenticationOptions options)
    {
        OnConfigureAuthentification(options);
    }

    /// <summary>
    /// Configure the authentication.
    /// </summary>
    /// <param name="options">The authentication options.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected virtual void OnConfigureAuthentification(AuthenticationOptions options) { }

    ///<inheritdoc/>
    private void RegisterAuthentication(
        IServiceCollection services, IConfiguration config, AuthenticationOptions authenticationOptions)
    {
        // We don't add authentication if authenticationOptions is null
        if (authenticationOptions is null)
        {
            return;
        }
        // Register the options as service
        services.AddSingleton(authenticationOptions);
        services.AddScoped<ILgAuthHelper, LgAuthHelper<TUser>>();
        // Enable Windows authentication
        if (authenticationOptions.AllowWindowsAuthentication)
        {
            // Configure both InProc & OutOfProc hosting model
            services.Configure<IISServerOptions>(options => options.AutomaticAuthentication = false);
            services.Configure<IISOptions>(options => options.AutomaticAuthentication = false);
        }
        services
            .AddAuthentication(o =>
            {
                o.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                o.DefaultAuthenticateScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            })
            .AddIdentityCookies(o =>
            {
                o.ApplicationCookie.Configure(c =>
                {
                    c.LoginPath = authenticationOptions.LoginUrl; // "/Identity/Account/Login";
                    c.Cookie.Name = $".{ApplicationInformation.RootName}.Identity";
                    // Rq: The Expire is managed when signin the user according to authentication mode (Form vs Windows/SSO)
                    // c.ExpireTimeSpan = ;
                });
            });

        // Add Identity Core with Roles, SignIn, Store & default token provider
        IdentityBuilder identityBuilder = services
            .AddIdentityCore<TUser>(o => ConfigureIdentity(authenticationOptions, o));
        GetUserKeyAdpter().ConfigureIdentity(identityBuilder);
        identityBuilder.AddSignInManager()
            .AddEntityFrameworkStores<TDbContext>()
            .AddDefaultTokenProviders();
        services.AddScoped<AuthenticationManager<TUser>>();
        services.AddScoped<IAuthenticationManager>(sp => sp.GetRequiredService<AuthenticationManager<TUser>>());
        // Configure Identity to use the same JWT claims as OpenIddict instead
        // of the legacy WS-Federation claims it uses by default (ClaimTypes),
        // which saves you from doing the mapping in your authorization controller.
        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserNameClaimType = Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = Claims.Role;
            options.ClaimsIdentity.EmailClaimType = Claims.Email;
        });
        // Register the OpenIddict core components.
        services.AddOpenIddict()
           .AddCore(ConfigureOpenIddictCore)
            // Register the OpenIddict server components.
            .AddServer(options =>
            {
                // Set issuer explicitly if configured in appSettings
                if (!string.IsNullOrEmpty(authenticationOptions.IssuerUri))
                {
                    options.SetIssuer(new Uri(authenticationOptions.IssuerUri));
                }

                // Set the token life time
                options.SetAccessTokenLifetime(TimeSpan.FromSeconds(authenticationOptions.AccessTokenLifetime));

                // Declare the authorization, logout, token and userinfo endpoints.
                options.SetAuthorizationEndpointUris("/connect/authorize")
                        .SetLogoutEndpointUris("/connect/logout")
                        .SetTokenEndpointUris("/connect/token")
                        .SetUserinfoEndpointUris("/connect/userinfo");

                // Mark the "email", "profile" and "roles" scopes as supported scopes.
                options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

                // Enable uthentication flow : code + refresh token + client credentials
                options.AllowAuthorizationCodeFlow()
                         .AllowRefreshTokenFlow()
                        .AllowClientCredentialsFlow();

                #region Key management

                string explicitCertificate = config.GetValue<string>("IdentityServer:Key:FilePath", null);
                if (string.IsNullOrEmpty(explicitCertificate))
                {
                    explicitCertificate = config.GetValue<string>("Lagoon:Authentication:OpendIdCertificate", null);
                }
                if (!string.IsNullOrEmpty(explicitCertificate))
                {
                    // Load the specified certificate for both Signing and Encryption
                    X509Certificate2 x509 = new(File.ReadAllBytes(explicitCertificate));
                    options
                        .AddEncryptionCertificate(x509)
                        .AddSigningCertificate(x509);
                }
                else
                {
                    // Register the signing and encryption credentials, both stored in the certificates store of the user account.
                    // Persisted when host is restarted (but can't be shared across instances)
                    string appName = ApplicationInformation.RootName;
                    options
                        .AddDevelopmentEncryptionCertificate(new X500DistinguishedName($"CN={appName} Encryption Certificate"))
                        .AddDevelopmentSigningCertificate(new X500DistinguishedName($"CN={appName} Server Signing Certificate"));
                }

                // Use .Net DataProtection have some advantages
                // => a little bit shorter and good perf (see https://documentation.openiddict.com/configuration/token-formats.html)
                // => easly configurable (see PersistKeysToFileSystem(), PersistKeysToAWSSystemsManager() or PersistKeysToAzureBlobStorage())
                // => automatic key new (90 days by default)
                options.UseDataProtection(dp =>
                {
                    //dp.PreferDefaultAccessTokenFormat();// => desactive le chiffrement par dataprotection du token
                    // (rq: mais maintenant contrairement a identity server le contenu de l'access_token est maintenant chiffré par le certif, donc autant la remplacer par la couche dataprotection)
                });

                #endregion
                // Register the ASP.NET Core host and enable the authorization, logout, token and userinfo endpoints
                OpenIddictServerAspNetCoreBuilder aspnetCoreOptions = options.UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableLogoutEndpointPassthrough()
                        .EnableStatusCodePagesIntegration()
                        .EnableTokenEndpointPassthrough();
                // Allow the use of HTTP if needed
                if (ApplicationInformation.PublicURL.StartsWith("http://"))
                {
                    aspnetCoreOptions.DisableTransportSecurityRequirement();
                }
            })

            // Register the OpenIddict validation components.
            .AddValidation(options =>
            {

                // Import the configuration from the local OpenIddict server instance.
                options.UseLocalServer();
                // Register the ASP.NET Core host.
                options.UseAspNetCore();
                // Register custom events
                options.AddEventHandler<OpenIddictValidationEvents.ProcessAuthenticationContext>(builder =>
                {
                    // Custom handler used to retrieve a token from a cookie
                    builder.UseSingletonHandler<TokenFromCookieHandler>();
                    builder.SetOrder(ExtractAccessTokenFromAuthorizationHeader.Descriptor.Order + 1);
                });
                // Use .Net DataProtection
                options.UseDataProtection();
            });
        // Register the default DataProtection
        if (authenticationOptions.UseDefaultDataProtection)
        {
            IDataProtectionBuilder dataProtection = services.AddDataProtection();
            string dataProtectionFolder = config["Lagoon:Authentication:KeyStoreDirectoryPath"];
            if (!string.IsNullOrWhiteSpace(dataProtectionFolder))
            {
                dataProtection.PersistKeysToFileSystem(new DirectoryInfo(dataProtectionFolder));
            }
        }
        // Application roles & policies
        services.AddAuthorization(options => ConfigureAuthorization(authenticationOptions, options));
    }

    /// <summary>
    /// Define the identity system options.
    /// </summary>
    /// <param name="authenticationOptions">Authentication options.</param>
    /// <param name="identityOptions">Represents all the options you can use to configure the identity system.</param>
    private static void ConfigureIdentity(AuthenticationOptions authenticationOptions, IdentityOptions identityOptions)
    {
        identityOptions.Stores.MaxLengthForKeys = 128;
        identityOptions.SignIn = authenticationOptions.SignInOptions;
        identityOptions.Password = authenticationOptions.PasswordOptions;
        identityOptions.User.AllowedUserNameCharacters = authenticationOptions.AllowedUserNameCharacters;
    }

    /// <summary>
    /// Registers the OpenIddict core services in the DI container.
    /// </summary>
    /// <param name="options">Options to configure the core services.</param>
    private static void ConfigureOpenIddictCore(OpenIddictCoreBuilder options)
    {
        // Configure OpenIddict to use the Entity Framework Core stores and models
        OpenIddictEntityFrameworkCoreBuilder openIddictOptions = options.UseEntityFrameworkCore()
            .UseDbContext<TDbContext>();
        GetUserKeyAdpter().ConfigureOpenIddictEFCore(openIddictOptions);
        // Enable Quartz.NET integration.
        options.UseQuartz();
    }

    /// <summary>
    /// Define the authorization options.
    /// </summary>
    /// <param name="authenticationOptions">Authentication options.</param>
    /// <param name="authorizationOptions">Authorization options.</param>
    protected internal virtual void ConfigureAuthorization(AuthenticationOptions authenticationOptions, AuthorizationOptions authorizationOptions)
    {
        OnConfigureAuthorization(authorizationOptions);
    }

    /// <summary>
    /// Define the authorization options.
    /// </summary>
    /// <param name="options">Authorization options.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected internal virtual void OnConfigureAuthorization(AuthorizationOptions options) { }

    #endregion

    #region database configuration and update

    /// <summary>
    /// Apply pending migrations and register PublicUrl in OpenIddict tables.
    /// </summary>
    /// <param name="scopedServiceProvider">A scoped service provider.</param>
    /// <param name="db">The DbContext of the application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected override async Task InternalUpdateDatabaseAsync(IServiceProvider scopedServiceProvider, TDbContext db, CancellationToken cancellationToken)
    {
        // Apply the pending migrations
        await base.InternalUpdateDatabaseAsync(scopedServiceProvider, db, cancellationToken);
        // Register the application PublicURL in OpenIddict tables
        await InitializeOpenIddictAsync(scopedServiceProvider, cancellationToken);
    }

    #endregion

    #region OpenIddict initialisation

    ///<inheritdoc/>
    Task ILgAuthApplication.InitializeOpenIddictAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        return InitializeOpenIddictAsync(serviceProvider, cancellationToken);
    }


    /// <summary>
    /// Initialize the OpenIddict application manager for Blazor hosted application (eg. the authorization server co-exist with the APIs)
    /// </summary>
    /// <param name="serviceProvider">A scoped service provider.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    internal Task InitializeOpenIddictAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        ILgOpenIddictManager openIddictManager = serviceProvider.GetRequiredService<ILgOpenIddictManager>();
        return openIddictManager.InitializeOpenIddictAsync(serviceProvider, ApplicationInformation, cancellationToken);
    }

    #endregion

    #region Authentication management

    /// <summary>
    /// Method to use for impersonate a user during development (login page bypass).
    /// </summary>
    /// <remarks>The base implementation return <c>null</c>.</remarks>
    public virtual IdentitySource GetImpersonatedIdentity()
    {
        return null;
    }

    /// <summary>
    /// SignIn user from login / password screen.
    /// </summary>
    /// <param name="login">User login</param>
    /// <param name="password">User password</param>
    /// <param name="rememberMe">If true, store authentication claims, else the user have to login every time</param>
    public Task<LgAuthenticationState> SignInAsync(string login, string password, bool rememberMe)
    {
        return SignInAsync(new FormsIdentitySource(login, password, rememberMe));
    }

    /// <summary>
    /// SignIn an user. 
    /// </summary>
    /// <param name="identitySource">User identity</param>
    public async Task<LgAuthenticationState> SignInAsync(IdentitySource identitySource)
    {
        using (IServiceScope scope = ServiceScopeFactory.CreateScope())
        {
            bool newUser = false;
            // Retrieve all services required to authenticate and sign an user
            InternalAuthenticationContext context = new(scope.ServiceProvider, identitySource);
            AuthenticationManager<TUser> AuthenticationManager = context.Manager;
            UserManager<TUser> userManager = context.UserManager;
            AuthenticationOptions authOptions = context.Options;
            // Allow to change the context configuration
            InternalConfigureAuthenticationContext(context);
            // Give the possibility to impersonate a user (Kept for compatibilty but to replace by GetImpersonatedIdentity()...)
            OnImpersonate(identitySource);
            // Get user informations (avec appel de OnInitUser)
            TUser user = await GetIdentityUserAsync(context);
            // In Windows & SSO authentication mode, allow the app to create a user at login time
            if (user is null && context.AllowUnknownUser)
            {
                user = await OnUnknownUserAsync(context);
                // rq: the user has been created in antoher db context so we have to attach the entity to the current db context
                if (user is not null)
                {
                    context.DbContext.Attach(user);
                    newUser = true;
                }
            }
            // Stop login progress if no user found
            if (user is null)
            {
                throw new AuthenticationException("InvalidLoginAttempt".Translate());
            }
            // Delegate auth to application
            OnSignIn(context, user);
            await OnSignInAsync(context, user);
            // Detect and save changes made to the user properties
            if (context.DbContext.Entry(user).State == EntityState.Modified)
            {
                await context.DbContext.SaveChangesAsync();
            }
            // User role management
            IList<string> userRoles;
            if (context.IsCustomRolesEnabled)
            {
                // Roles update
                userRoles = context.CustomsRoles;
                if (userRoles is null || !userRoles.Any())
                {
                    throw new AuthenticationException("AuthCustomRole".Translate());
                }
                // Clear existing user roles
                await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));
                // Add new roles
                (await userManager.AddToRolesAsync(user, userRoles)).EnsureSuccess("AuthCantAddUserRoles".Translate());
            }
            else
            {
                userRoles = await userManager.GetRolesAsync(user);
                // Check if user has one role at least                
                if (!userRoles.Any())
                {
                    throw new AuthenticationException("AuthOneRoleAtLeast".Translate());
                }
            }
            // Adds the choice among the user's roles
            if (context.GroupChoiceEnumType is not null)
            {
                foreach (string roleName in userRoles)
                {
                    DisplayAttribute attribute = context.GroupChoiceEnumType.GetField(roleName, BindingFlags.IgnoreCase)?.GetCustomAttribute<DisplayAttribute>();
                    string description = attribute is null ? roleName : attribute.GetName().CheckTranslate();
                    identitySource.AddRoleChoice(description, roleName);
                }
            }
            // Claims update
            List<Claim> claims = context.CustomsClaims;
            if (claims is not null && claims.Any())
            {
                // Clear user claims
                await userManager.RemoveClaimsAsync(user, await userManager.GetClaimsAsync(user));
                // Add new claims
                (await userManager.AddClaimsAsync(user, claims)).EnsureSuccess("AuthCantAddUserClaims".Translate());
            }
            // Validate user credential
            await identitySource.CheckUserAccessAsync(user, userManager);
            // If MFA must be enabled for all account or for users in 'MultiFactorGroupName' group
            if ((authOptions.MultiFactorAuthenticationRequired && !user.TwoFactorEnabled) ||
                (!string.IsNullOrEmpty(authOptions.MultiFactorAuthenticationRoleName) && !user.TwoFactorEnabled && await userManager.IsInRoleAsync(user, authOptions.MultiFactorAuthenticationRoleName)))
            {
                // Temporary sign the user on TwoFactor scheme
                await AuthenticationManager.SignInTwoFactorAsync(user, identitySource.SourceName);
                return LgAuthenticationState.RequireMfaActivation;
            }
            else if (user.TwoFactorEnabled && !await AuthenticationManager.IsTwoFactorClientRememberedAsync(user))
            {
                // Temporary sign the user
                await AuthenticationManager.SignInTwoFactorAsync(user, identitySource.SourceName);
                return LgAuthenticationState.RequireMfaValidation;
            }
            else
            {
                // The expiration time of the authentication cookie depend on authentication mode
                // For "Forms" authentication we can store the cookie for a long time
                // but for "Windows" and "SSO" authentication we should set a shorter validity perdiod to check if
                // the user is still in the AD/SSO
                // Rq: Application roles (eg. user role managed by the application itself) are checked for each token generation 
                // (see AuthorizationController, ExchangeAsync method & ValidateSecurityStampAsync use)
                AuthenticationProperties signOptions = new()
                {
                    IsPersistent = identitySource.RememberMe,
                    ExpiresUtc = identitySource.AuthenticationMode == AuthenticationMode.Forms
                    ? DateTimeOffset.UtcNow.AddDays(authOptions.FormAuthenticationCookieLifeTime)
                    : DateTimeOffset.UtcNow.AddDays(authOptions.SsoAuthenticationCookieLifeTime)
                };
                // When using an SSO we can request additionnal tokens which can be used to get access on behalf of a user
                // ExternalLoginInfo.AuthenticationTokens are provided when RemoteAuthenticationOptions.SaveTokens is set to true
                // on registered external authentication provider (eg service.AddAuthentication().AddOpenIdConnect(...))
                if (identitySource.ExternalLoginInfo is not null && identitySource.ExternalLoginInfo.AuthenticationTokens.Any())
                {
                    // We always try to associate the external info provided by the SSO to an application user in case of an existing account
                    // (connected with a different authentication provider so it's not "new" user). The AddLoginAsync will just throw a warning :
                    // "AddLogin for user failed because it was already associated with another user."
                    //if (newUser)
                    {
                        // Map the user to the external login provider. So we can later retrieving these info with an application user
                        await userManager.AddLoginAsync(user, identitySource.ExternalLoginInfo);
                    }
                    // When logged with an SSO the tokens are refreshed and we have to update them
                    await AuthenticationManager.UpdateExternalAuthenticationTokensAsync(identitySource.ExternalLoginInfo);
                    signOptions.StoreTokens(identitySource.ExternalLoginInfo.AuthenticationTokens);
                }
                // Sign in the user
                await AuthenticationManager.SignInAsync(user, signOptions);
                return LgAuthenticationState.Success;
            }
        }
    }


    /// <summary>
    /// Signout the current user.
    /// </summary>
    public async Task SignOutAsync()
    {
        using (IServiceScope scope = ServiceScopeFactory.CreateScope())
        {
            SignInManager<TUser> signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<TUser>>();
            await signInManager.SignOutAsync();
        }
    }

    /// <inheritdoc />
    public virtual async Task<string> GenerateResetPasswordLinkAsync(string userEmail)
    {
        using (IServiceScope scope = ServiceScopeFactory.CreateScope())
        {
            // Retrieve the account
            UserManager<TUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
            TUser appUser = await userManager.FindByEmailAsync(userEmail);
            if (userEmail != null)
            {
                // Generate password reset code 
                string code = await userManager.GeneratePasswordResetTokenAsync(appUser);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                return $"/Identity/Account/ResetPassword?userId={appUser.Id}&code={code}";
            }
        }
        throw new InvalidOperationException($"Unable to generate a password reset link for {userEmail}");
    }

    /// <inheritdoc />
    public virtual async Task<IdentityResult> ResetPasswordAsync(string userId, string code, string password)
    {

        using (IServiceScope scope = ServiceScopeFactory.CreateScope())
        {
            // Retrieve the account
            UserManager<TUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
            TUser user = await userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                return await userManager.ResetPasswordAsync(user, code, password);
            }
        }
        throw new InvalidOperationException($"Unable to reset password for {userId}");
    }

    /// <inheritdoc />
    public async Task<IdentityResult> ChangePasswordAsync(ClaimsPrincipal userPrincipal, string actualPassword, string newPassword)
    {
        using (IServiceScope scope = ServiceScopeFactory.CreateScope())
        {
            // Retrieve the account
            UserManager<TUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
            TUser user = await userManager.GetUserAsync(userPrincipal);
            if (!await userManager.CheckPasswordAsync(user, actualPassword))
            {
                return IdentityResult.Failed(new IdentityError() { Code = "BadPassword", Description = "The given password not match the actual user password" });
            }
            if (user is not null)
            {
                user.PasswordHash = userManager.PasswordHasher.HashPassword(user, newPassword);
                return await userManager.UpdateAsync(user);
            }
        }
        throw new InvalidOperationException($"Unable to change password for {userPrincipal?.Identity.Name}");
    }

    /// <summary>
    /// Method called while the authentication context is configured.
    /// </summary>
    /// <param name="context">The current authentication context.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void InternalConfigureAuthenticationContext(AuthenticationContext context)
    {
        OnConfigureAuthenticationContext(context);
    }

    /// <summary>
    /// Method called while the authentication context is configured.
    /// </summary>
    /// <param name="context">The current authentication context.</param>
    /// <remarks>The base method does nothing.</remarks>
    protected virtual void OnConfigureAuthenticationContext(AuthenticationContext context)
    { }

    /// <summary>
    /// Give the possibility to create an user at sign-in time.
    /// </summary>
    /// <param name="context">The current authentication context.</param>
    /// <returns>A new application user; <c>null</c> to cancel the sign-in process.</returns>
    /// <remarks>Replace "OnUnknownUser" by "OnUnknownUserAsync".</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Replace \"OnUnknownUser\" by \"OnUnknownUserAsync\".")] //06/03/2023
    protected virtual Task<TUser> OnUnknownUser(AuthenticationContext context)
    {
        return OnUnknownUserAsync(context);
    }

    /// <summary>
    /// Give the possibility to create an user at sign-in time.
    /// </summary>
    /// <param name="context">The current authentication context.</param>
    /// <returns>A new application user; <c>null</c> to cancel the sign-in process.</returns>
    /// <remarks>The base implementation return <c>null</c>.</remarks>
    protected virtual Task<TUser> OnUnknownUserAsync(AuthenticationContext context)
    {
        return Task.FromResult<TUser>(null);
    }

    /// <summary>
    /// Authentication impersonation endpoint.
    /// </summary>
    /// <param name="identity">User currently login.</param>        
    /// <remarks>OBSOLETE: Use the GetImpersonatedIdentity() method.</remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual void OnImpersonate(IdentitySource identity)
    { }

    /// <summary>
    /// Add roles to the <paramref name="context.IdentitySource"/>> for the sign-in user <paramref name="user"/>.
    /// </summary>
    /// <param name="context">The current authentication context.</param>
    /// <param name="user">The application user.</param>
    protected virtual Task OnSignInAsync(AuthenticationContext context, TUser user)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Add roles to the <paramref name="context.IdentitySource"/>> for the sign-in user <paramref name="user"/>.
    /// </summary>
    /// <param name="context">The current authentication context.</param>
    /// <param name="user">The application user.</param>
    protected virtual void OnSignIn(AuthenticationContext context, TUser user)
    { }

    /// <summary>
    /// Find the corresponding user with identitySource information in the database.
    /// </summary>
    /// <param name="context">The current authentication context.</param>
    /// <returns>Application user if found, null otherwise</returns>
    protected virtual Task<TUser> GetIdentityUserAsync(AuthenticationContext context)
    {
        return context.IsEmailLogin
            ? context.UserManager.FindByEmailAsync(context.IdentitySource.UserLogin)
            : context.UserManager.FindByNameAsync(context.IdentitySource.UserLogin);
    }

    #endregion

    #region User management (helpers methods)


    /// <summary>
    /// Get the adadpter to work with the generic type key of the user id.
    /// </summary>
    /// <returns>The adadpter to work with the generic type key of the user id.</returns>
    private static IUserKeyAdapter GetUserKeyAdpter()
    {
        Type type = typeof(TUser);
        while (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(IdentityUser<>))
        {
            type = type.BaseType;
        }
        type = type.GetGenericArguments()[0];
        return (IUserKeyAdapter)Activator.CreateInstance(typeof(KeyAdapter<>).MakeGenericType(typeof(TDbContext), typeof(TUser), type));
    }


    /// <summary>
    /// Add an user in application database
    /// </summary>
    /// <param name="user">User to add</param>
    /// <param name="role">The user role.</param>
    /// <param name="claims">The user additionnal claims.</param>
    /// <returns>Application user</returns>
    public Task<TUser> CreateUserAsync<TRole>(TUser user, TRole role, params Claim[] claims) where TRole : Enum
    {
        return CreateUserAsync(user, new string[] { role.ToString() }, claims);
    }

    /// <summary>
    /// Add an user in application database
    /// </summary>
    /// <param name="user">User to add</param>
    /// <param name="roles">User roles</param>
    /// <returns>Application user</returns>
    public Task<TUser> CreateUserAsync<TRole>(TUser user, params TRole[] roles) where TRole : Enum
    {
        return CreateUserAsync(user, roles?.Select(r => r.ToString()), null);
    }

    /// <summary>
    /// Add an user in application database
    /// </summary>
    /// <param name="user">User to add</param>
    /// <param name="roles">User roles</param>
    /// <param name="claims">Optional user claims</param>
    /// <returns>Application user</returns>
    public async Task<TUser> CreateUserAsync(TUser user, IEnumerable<string> roles, IEnumerable<Claim> claims = null)
    {
        using (IServiceScope scope = ServiceScopeFactory.CreateScope())
        {
            // Save user
            UserManager<TUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
            (await userManager.CreateAsync(user)).EnsureSuccess("AuthCantCreateUser".Translate());
            (await userManager.AddToRolesAsync(user, roles)).EnsureSuccess("AuthCantAddUserRoles".Translate());
            if (claims != null)
            {
                (await userManager.AddClaimsAsync(user, claims)).EnsureSuccess("AuthCantAddUserClaims".Translate());
            }
        }
        return user;
    }

    /// <summary>
    /// Add an user in application database
    /// </summary>
    /// <param name="user">User to add</param>
    /// <param name="claims">Optional user claims</param>
    /// <param name="roles">User roles</param>
    /// <returns>Application user</returns>
    public Task<TUser> CreateUserAsync<TRole>(TUser user, IEnumerable<Claim> claims, params TRole[] roles) where TRole : Enum
    {
        return CreateUserAsync(user, roles?.Select(r => r.ToString()), claims);
    }

    #endregion

}
