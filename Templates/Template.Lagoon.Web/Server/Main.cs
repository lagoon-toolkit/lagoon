using Lagoon.Server.Application;
using Lagoon.Server.Application.IdentitySources;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TemplateLagoonWeb.Model.Models;
using TemplateLagoonWeb.Model.Seeds;

namespace TemplateLagoonWeb.Server;

/// <summary>
/// The web server application's configuration and event manager.
/// </summary>
public class Main : LgAuthApplication<ApplicationDbContext, ApplicationUser>
{

    #region Main Database configuration

    /// <summary>
    /// Called when the main application DbContext need to be configured.
    /// </summary>
    /// <param name="builder">A builder used to create or modify options for the main database context.</param>
    protected override void OnConfigureDbContext(DbContextOptionsBuilder builder)
    {
        ApplicationDbContext.Configure(builder, Configuration, ApplicationInformation.IsDevelopment);
    }

    #endregion

    #region Services registration

    /// <summary>
    /// Register the services for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <remarks>The base implementation does nothing.</remarks>
    protected override void OnConfigureServices(IServiceCollection services)
    {
        // Register services defined in this application
        Services.Registry.RegisterServices(services);
    }

    #endregion

    #region Application startup

    /// <summary>
    /// Method called when the database is updating with "UpdateDatabaseAsync()"
    /// or if "Lagoon:AutoUpdateDatabase" parameter is "true" or undefined in the "appsettings.json" file.
    /// </summary>
    /// <param name="scopedServiceProvider">A scoped service provider.</param>
    /// <param name="db">The application's DbContext.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    protected override async Task OnUpdateDatabaseAsync(IServiceProvider scopedServiceProvider, ApplicationDbContext db, CancellationToken cancellationToken)
    {
        // Initialize the data
        UserManager<ApplicationUser> userManager = scopedServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await DataSeeder.SeedAsync(db, userManager, cancellationToken);
//-:cnd:noEmit
#if DEBUG
//+:cnd:noEmit
        // Fill the database with fake data for development tests
        if (ApplicationInformation.IsDevelopment)
        {
            await FakeDataSeeder.SeedAsync(db, cancellationToken);
        }
//-:cnd:noEmit
#endif
//+:cnd:noEmit
    }

    #endregion

    #region Pipeline configuration

    /// <summary>
    /// Configure the HTTP request pipeline for the application.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    protected override void OnConfigure(IApplicationBuilder app)
    {
        // Enable debugging of Blazor WebAssembly applications in Chromium development tools
        if (ApplicationInformation.IsDevelopment)
        {
            app.UseWebAssemblyDebugging();
        }
        // Configure HTTP request handling
        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();
        app.UseRouting();
        // Enable authentication capabilities
        app.UseAuthentication();
        app.UseAuthorization();
        // Map endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            endpoints.MapLagoonFallback();
        });
    }

    #endregion

    #region User authentication configuration

    /// <summary>
    /// Configure the authentication.
    /// </summary>
    /// <param name="options">The authentication options.</param>
    protected override void OnConfigureAuthentification(AuthenticationOptions options)
    {
        // Activate the form authentication
        options.AllowFormAuthentication = true;
    }

    /// <summary>
    /// Define the authorization options.
    /// </summary>
    /// <param name="options">Authorization options.</param>
    protected override void OnConfigureAuthorization(AuthorizationOptions options)
    {
        // Register policies
        Policies.ConfigureAuthorization(options);
    }

    #endregion

    #region User authentication workflow

//-:cnd:noEmit
#if DEBUG
//+:cnd:noEmit

    /// <summary>
    /// Method used to impersonate a user (login page is bypassed).
    /// </summary>
    /// <remarks>The base implementation return <c>null</c>.</remarks>
    public override IdentitySource GetImpersonatedIdentity()
    {
        return ImpersonatedIdentity.FromConfiguration(Configuration);
    }

//-:cnd:noEmit
#endif
//+:cnd:noEmit

    /// <summary>
    /// Method called while the authentication context is configured.
    /// </summary>
    /// <param name="context">The current authentication context.</param>
    /// <remarks>The base method does nothing.</remarks>
    protected override void OnConfigureAuthenticationContext(AuthenticationContext context)
    {
        // Sets if the "OnUnknownUserAsync" will be called
        context.AllowUnknownUser = context.IdentitySource.AuthenticationMode != AuthenticationMode.Forms;
        // Sets if we identify users by the "Email" or by the "UserName" property
        context.IsEmailLogin = context.IdentitySource.AuthenticationMode == AuthenticationMode.SSO;
        // Allow the user to select only one of his roles during the connection
        context.EnableGroupChoice<Roles>();
    }

    /// <summary>
    /// Creates a user at login time when a user is identified by an SSO identity provider.
    /// </summary>
    /// <param name="context">The current authentication context.</param>
    /// <returns>A new application user; <c>null</c> to cancel the sign-in process.</returns>
    /// <remarks>The base implementation return <c>null</c>.</remarks>
    protected override Task<ApplicationUser> OnUnknownUserAsync(AuthenticationContext context)
    {
        // The unknown user will not be created
        return Task.FromResult<ApplicationUser>(null);
    }

    /// <summary>
    /// User signin attempt.
    /// </summary>
    /// <param name="identity">The connection informations.</param>
    /// <param name="user">The user informations.</param>
    protected override Task OnSignInAsync(AuthenticationContext context, ApplicationUser user)
    {
        return Task.CompletedTask;
    }

    #endregion

}
